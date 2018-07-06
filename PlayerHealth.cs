using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{

	public float maxHealth;
	public GameObject deathPrefab;
	[SyncVar]
	public bool isDead = false;
	public RectTransform healthBar;

	public PlayerControl lastAttacker;

	[SyncVar(hook = "UpdateHealthBar")]
	private float currentHealth;
	private float healthBarXSize;

	// Use this for initialization

	private void Awake()
	{
		healthBarXSize = healthBar.sizeDelta.x;
	}

	void Start () {

		Reset();
		

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TakeDamage(float damage, PlayerControl pc = null)
	{

		if (!isServer)
			return;

		if(pc != null && pc != this.GetComponent<PlayerControl>())
		{
			lastAttacker = pc;
		}

		currentHealth -= damage;
		
		if(currentHealth <= 0 && !isDead)
		{

			if(lastAttacker != null)
			{
				lastAttacker.score++;
				lastAttacker = null;
			}

			//GameManager.instance.UpdateScore();

			isDead = true;
			RpcDie();
		}
	}

	void UpdateHealthBar(float value)
	{
		healthBar.sizeDelta = new Vector2(value / maxHealth * healthBarXSize, healthBar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcDie()
	{
		GameObject deathFX = Instantiate(deathPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
		Destroy(deathFX, 3f);

		SetActiveState(false);

		gameObject.SendMessage("Disable");

	}

	void SetActiveState(bool state)
	{
		foreach (var c in GetComponentsInChildren<Collider>())
		{
			c.enabled = state;
		}

		foreach (var c in GetComponentsInChildren<Canvas>())
		{
			c.enabled = state;
		}

		foreach (var r in GetComponentsInChildren<Renderer>())
		{
			r.enabled = state;
		}
	}

	public void Reset()
	{
		currentHealth = maxHealth;
		SetActiveState(true);
		isDead = false;
	}
}
