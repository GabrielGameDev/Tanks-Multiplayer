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
	}

	IEnumerator EnterLobby()
	{
		messageText.gameObject.SetActive(true);
		messageText.text = "Esperando jogadores...";
		
		while (playerCount < minPlayers)
		{
			DisablePlayers();
			yield return null;
		}

		
	}

	IEnumerator PlayGame()
	{
		EnablePlayers();
		UpdateScore();
		messageText.gameObject.SetActive(false);
		yield return null;
	}

	IEnumerator EndGame()
	{
		yield return null;
	}

	void SetPlayerState(bool state)
	{
		PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
		foreach (PlayerControl p in allPlayers)
		{
			p.enabled = state;
		}
	}

	void EnablePlayers()
	{
		SetPlayerState(true);
	}

	void DisablePlayers()
	{
		SetPlayerState(false);
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
}
