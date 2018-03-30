using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

	public Rigidbody bulletPrefab;
	public float bulletSpeed = 20;
	public float fireRate = 0.5f;
	public Transform bulletSpawn;

	private float nextFire;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[Command]
	public void CmdShoot()
	{
		if(Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;
			Rigidbody tempBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
			tempBullet.velocity = bulletSpeed * bulletSpawn.transform.forward;

			NetworkServer.Spawn(tempBullet.gameObject);
		}
	}
}
