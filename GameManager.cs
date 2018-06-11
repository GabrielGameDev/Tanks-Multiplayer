using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

	public static GameManager instance;

	public Text messageText;

	public int minPlayers = 2;
	public int maxPlayers = 4;

	[SyncVar]
	public int playerCount = 0;

	public List<PlayerControl> allPlayers;
	public List<Text> nameText;
	public List<Text> playerScoreText;

	public Color[] playerColors;

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

		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {

		StartCoroutine(GameLoopRoutine());

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator GameLoopRoutine()
	{
		yield return StartCoroutine(EnterLobby());
		yield return StartCoroutine(PlayGame());
		yield return StartCoroutine(EndGame());
		StartCoroutine(GameLoopRoutine());
	}

	IEnumerator EnterLobby()
	{
		
		
		while (playerCount < minPlayers)
		{
			UpdateMessage("Esperando jogadores...");
			DisablePlayers();
			yield return null;
		}

		
	}

	IEnumerator PlayGame()
	{
		UpdateMessage("");
		EnablePlayers();
		UpdateScore();
		while (!gameOver)
		{
			yield return null;
		}
	}

	IEnumerator EndGame()
	{
		DisablePlayers();
		UpdateMessage("GAME OVER \n" + winner.pSetup.playerNameText.text + " venceu!");
		Reset();
		yield return new WaitForSeconds(3f);
		UpdateMessage("Reiniciando...");
		yield return new WaitForSeconds(3f);

	}


	[ClientRpc]
	void RpcUpdateMessage(string msg)
	{
		messageText.gameObject.SetActive(true);
		messageText.text = msg;
	}

	public void UpdateMessage(string msg)
	{
		if (isServer)
		{
			RpcUpdateMessage(msg);
		}
	}

	[ClientRpc]
	void RpcSetPlayerState(bool state)
	{
		PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
		foreach (PlayerControl p in allPlayers)
		{
			p.enabled = state;
		}
	}

	void EnablePlayers()
	{
		if(isServer)
			RpcSetPlayerState(true);
	}

	void DisablePlayers()
	{
		if(isServer)
			RpcSetPlayerState(false);
	}

	public void AddPlayer(PlayerSetup pS)
	{
		if(playerCount < maxPlayers)
		{
			allPlayers.Add(pS.GetComponent<PlayerControl>());
			pS.playerColor = playerColors[playerCount];
			pS.playerNum = playerCount + 1;
			playerCount++;
		}
	}

	[ClientRpc]
	void RpcUpdateScore(int[] playerScores, string[] playerNames)
	{
		for (int i = 0; i < playerCount; i++)
		{
			playerScoreText[i].text = playerScores[i].ToString();
			nameText[i].text = playerNames[i];
		}
	}

	public void UpdateScore()
	{
		if (isServer)
		{
			winner = GetWinner();
			if(winner != null)
			{
				gameOver = true;
			}
			int[] scores = new int[playerCount];
			string[] names = new string[playerCount];
			for (int i = 0; i < playerCount; i++)
			{
				scores[i] = allPlayers[i].score;
				names[i] = allPlayers[i].GetComponent<PlayerSetup>().playerNameText.text;
			}

			RpcUpdateScore(scores, names);
		}
	}

	PlayerControl GetWinner()
	{
		if (isServer)
		{
			for (int i = 0; i < playerCount; i++)
			{
				if(allPlayers[i].score >= maxScore)
				{
					return allPlayers[i];
				}
			}
		}

		return null;
	}

	void Reset()
	{
		if (isServer)
		{
			RpcReset();
			UpdateScore();
			winner = null;
			gameOver = false;
		}
	}

	[ClientRpc]
	void RpcReset()
	{
		PlayerControl[] players = FindObjectsOfType<PlayerControl>();
		foreach (var player in players)
		{
			player.score = 0;
		}
	}
}
