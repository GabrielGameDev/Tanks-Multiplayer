﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour {

	public ParticleSystem explosionFX;
	public int bounces = 2;

	private Rigidbody rb;
	private Collider col;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Explode()
	{

		rb.velocity = Vector3.zero;
		rb.Sleep();
		col.enabled = false;

		foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
		{
			mesh.enabled = false;
		}

		foreach (var ps in GetComponentsInChildren<ParticleSystem>())
		{
			ps.Stop();
		}

		explosionFX.transform.parent = null;
		explosionFX.Play();

		Destroy(explosionFX.gameObject, 1);

		Destroy(gameObject);

	}

	private void OnCollisionExit(Collision collision)
	{
		if(rb.velocity != Vector3.zero)
		{
			transform.rotation = Quaternion.LookRotation(rb.velocity);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{

		PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
		if(player != null)
		{
			Explode();
			player.TakeDamage(1);
		}

		if (bounces <= 0)
			Explode();

		bounces--;
	}

}
