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

	public Text playerNameText;

	

	public override void OnStartClient()
	{
		base.OnStartClient();

		if (!isServer)
		{
			PlayerControl pControl = GetComponent<PlayerControl>();
			if (pControl != null)
			{
				GameManager.allPlayers.Add(pControl);
			}
		}

		UpdateName(baseName);
		UpdateColor(playerColor);
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
