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

	public void AddPlayer()
	{
		if(playerCount < maxPlayers)
		{
			playerCount++;
		}
	}
}
