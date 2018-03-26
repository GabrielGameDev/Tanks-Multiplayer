using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{

	public Color playerColor;
	public string baseName = "PLAYER";
	public int playerNum = 1;
	public Text playerNameText;

	// Use this for initialization
	void Start () {
		
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

		MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer m in meshes)
		{
			m.material.color = playerColor;
		}
		playerNameText.enabled = true;
		playerNameText.text = baseName + " " + playerNum;
	}
}
