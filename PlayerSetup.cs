using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
	[SyncVar(hook = "UpdateColor")]
	public Color playerColor;
	[SyncVar(hook = "UpdateName")]
	public string baseName = "PLAYER";

	
	//public int playerNum = 1;
	public Text playerNameText;

	// Use this for initialization
	void Start () {

		UpdateName(baseName);
		UpdateColor(playerColor);


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		playerNameText.enabled = false;
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();

		CmdSetupPlayer();

		
	}

	[Command]
	void CmdSetupPlayer()
	{
		GameManager.instance.AddPlayer(this);
	}

	void UpdateColor(Color color)
	{
		MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer m in meshes)
		{
			m.material.color = color;
		}
	}

	void UpdateName(string nome)
	{
		playerNameText.enabled = true;
		playerNameText.text = nome;
	}
}
