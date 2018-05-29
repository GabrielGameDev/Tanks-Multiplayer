using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	public bool isOcupied = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isOcupied = true;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isOcupied = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isOcupied = false;
		}
	}
}
