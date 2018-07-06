using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{

	public GameObject spawnFX;

	[SyncVar]
	public int score;

	private PlayerMotor pMotor;
	private PlayerShoot pShoot;
	private PlayerHealth pHealth;
	public PlayerSetup pSetup;

	private NetworkStartPosition[] spawnPoints;
	private Vector3 originalPosition;

	// Use this for initialization
	void Start () {

		pMotor = GetComponent<PlayerMotor>();
		pShoot = GetComponent<PlayerShoot>();
		pHealth = GetComponent<PlayerHealth>();
		pSetup = GetComponent<PlayerSetup>();
	}

	public override void OnStartLocalPlayer()
	{
		spawnPoints = FindObjectsOfType<NetworkStartPosition>();
		originalPosition = transform.position;
	}

	// Update is called once per frame
	void Update () {

		if (!isLocalPlayer || pHealth.isDead)
			return;

		Vector3 inputDirection = GetInput();
		if(inputDirection.sqrMagnitude > 0.25f)
		{
			pMotor.RotateChassis(inputDirection);
		}

		Vector3 turretDir = Utility.GetWorldPointFromScreenPoint(Input.mousePosition, pMotor.turret.position.y) - pMotor.turret.position;
		pMotor.RotateTurret(turretDir);

		if (Input.GetMouseButton(0))
		{
			pShoot.CmdShoot();
		}

	}

	private void OnDestroy()
	{
		GameManager.allPlayers.Remove(this);
	}

	Vector3 GetInput()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		return new Vector3(h, 0, v);
	}

	private void FixedUpdate()
	{
		if (!isLocalPlayer || pHealth.isDead)
			return;

		pMotor.MovePlayer(GetInput());
	}

	void Disable()
	{
		StartCoroutine(Respawn());
	} 

	IEnumerator Respawn()
	{
		SpawnPoint oldSpawn = GetNearestSpawnPoint();
		if(oldSpawn != null)
		{
			oldSpawn.isOcupied = false;
		}

		transform.position = GetRandomSpawnPosition();
		pMotor.rb.velocity = Vector3.zero;
		yield return new WaitForSeconds(3f);
		pHealth.Reset();
		GameObject newSpawnFX = Instantiate(spawnFX, transform.position, Quaternion.identity);
		Destroy(newSpawnFX, 3f);

		EnableControls();
	}

	SpawnPoint GetNearestSpawnPoint()
	{
		Collider[] triggerColliders = Physics.OverlapSphere(transform.position, 3f, Physics.AllLayers, QueryTriggerInteraction.Collide);
		foreach (var c in triggerColliders)
		{
			SpawnPoint spawnpoint = c.GetComponent<SpawnPoint>();
			if(spawnpoint != null)
			{
				return spawnpoint;
			}
		}

		return null;
	}

	Vector3 GetRandomSpawnPosition()
	{
		if(spawnPoints != null)
		{
			bool foundSpawner = false;
			Vector3 newStartPosition = new Vector3();
			float timeOut = Time.time + 2f;

			while (!foundSpawner)

			{
				NetworkStartPosition startPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
				SpawnPoint spawnPoint = startPoint.GetComponent<SpawnPoint>();
				if(spawnPoint.isOcupied == false)
				{
					newStartPosition = startPoint.transform.position;
					foundSpawner = true;
				}

				if(Time.time > timeOut)
				{
					foundSpawner = true;
					newStartPosition = originalPosition;
				}
			}

			return newStartPosition;
		}

		return originalPosition;
	}

	public void EnableControls()
	{
		pMotor.Enable();
		pShoot.Enable();
	}

	public void DisableControls()
	{
		pMotor.Disable();
		pShoot.Disable();
	}
}
