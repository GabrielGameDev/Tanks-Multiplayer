﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

	private PlayerMotor pMotor;

	// Use this for initialization
	void Start () {

		pMotor = GetComponent<PlayerMotor>();

	}
	
	// Update is called once per frame
	void Update () {

		Vector3 inputDirection = GetInput();
		if(inputDirection.sqrMagnitude > 0.25f)
		{
			pMotor.RotateChassis(inputDirection);
		}

		Vector3 turretDir = Utility.GetWorldPointFromScreenPoint(Input.mousePosition, pMotor.turret.position.y) - pMotor.turret.position;
		pMotor.RotateTurret(turretDir);

	}

	Vector3 GetInput()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		return new Vector3(h, 0, v);
	}

	private void FixedUpdate()
	{
		pMotor.MovePlayer(GetInput());
	}
}