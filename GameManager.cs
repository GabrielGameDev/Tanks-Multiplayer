 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class GameManager : NetworkBehaviour {

	public static GameManager instance;

	public Text messageText;

	public static List<PlayerControl> allPlayers = new List<PlayerControl>();
	public List<Text> nameText;
	public List<Text> playerScoreText;

	public int maxScore = 3;

	[SyncVar]
	private bool gameOver = false;

	private PlayerControl winner;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}

	}

	[Server]
	void Start () {

		StartCoroutine(GameLoopRoutine());

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator GameLoopRoutine()
	{
		LobbyManager lManager = LobbyManager.s_Singleton;
		if(lManager != null)
		{
			while (allPlayers.Count < lManager._playerNumber)
			{
				yield return null;
			}
		}
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(StartGame());
		yield return StartCoroutine(PlayGame());
		yield return StartCoroutine(EndGame());
		StartCoroutine(GameLoopRoutine());
	}

	[ClientRpc]
	void RpcStartGame()
	{
		UpdateMessage("Lutem!");
		DisablePlayers();
	}

	IEnumerator StartGame()
	{

		Reset();
		RpcStartGame();
		UpdateScore();
		yield return new WaitForSeconds(2f);

		
	}

	[ClientRpc]
	void RpcPlayGame()
	{
		EnablePlayers();
		UpdateMessage("");
	}

	IEnumerator PlayGame()
	{
		RpcPlayGame();
		
		while (!gameOver)
		{
			CheckScores();
			yield return null;
		}
	}

	[ClientRpc]
	void RpcEndGame()
	{
		DisablePlayers();
	}

	IEnumerator EndGame()
	{
		RpcEndGame();
		RpcUpdateMessage("GAME OVER \n" + winner.pSetup.baseName + " venceu!");
		yield return new WaitForSeconds(3f);
		Reset();
		LobbyManager.s_Singleton._playerNumber = 0;
		LobbyManager.s_Singleton.SendReturnToLobby();
		

	}


	[ClientRpc]
	void RpcUpdateMessage(string msg)
	{
		UpdateMessage(msg);
	}

	public void UpdateMessage(string msg)
	{

		messageText.gameObject.SetActive(true);
		messageText.text = msg;
		
	}
		

	void EnablePlayers()
	{
		for (int i = 0; i < allPlayers.Count; i++)
		{
			if(allPlayers[i] != null)
			{
				allPlayers[i].EnableControls();
			}
		}
	}

	void DisablePlayers()
	{
		for (int i = 0; i < allPlayers.Count; i++)
		{
			if (allPlayers[i] != null)
			{
				allPlayers[i].DisableControls();
			}
		}
	}

	
	public void CheckScores()
	{
		winner = GetWinner();
		if(winner != null)
		{
			gameOver = true;
		}
	}

	
	[ClientRpc]
	void RpcUpdateScore(int[] playerScores, string[] playerNames)
	{
		for (int i = 0; i < allPlayers.Count; i++)
		{
			playerScoreText[i].text = playerScores[i].ToString();
			nameText[i].text = playerNames[i];
		}
	}

	public void UpdateScore()
	{
		if (isServer)
		{
			string[] pNames = new string[allPlayers.Count];
			int[] pScores = new int[allPlayers.Count];

			for (int i = 0; i < allPlayers.Count; i++)
			{
				if(allPlayers[i] != null)
				{
					pNames[i] = allPlayers[i].GetComponent<PlayerSetup>().baseName;
					pScores[i] = allPlayers[i].score;
				}
			}

			RpcUpdateScore(pScores, pNames);
		}
	}

	PlayerControl GetWinner()
	{
		
		for (int i = 0; i < allPlayers.Count; i++)
		{
			if(allPlayers[i].score >= maxScore)
			{
				return allPlayers[i];
			}
		}		

		return null;
	}

	void Reset()
	{
		for (int i = 0; i < allPlayers.Count; i++)
		{
			PlayerHealth pHealth = allPlayers[i].GetComponent<PlayerHealth>();
			pHealth.Reset();
			allPlayers[i].score = 0;
		}
	}

}
