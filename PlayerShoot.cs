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
	private bool canShoot = false;
	

	public void Enable()
	{
		canShoot = true;
	}

	public void Disable()
	{
		canShoot = false;
	}

	[Command]
	public void CmdShoot()
	{
		if (!canShoot)
		{
			return;
		}

		if(Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;
			Rigidbody tempBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
			tempBullet.velocity = bulletSpeed * bulletSpawn.transform.forward;
			tempBullet.GetComponent<Bullet>().owner = GetComponent<PlayerControl>();
			NetworkServer.Spawn(tempBullet.gameObject);

		}
	}
}
