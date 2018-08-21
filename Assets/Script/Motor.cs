using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour {
	public float moveSpeed = 5.0f;
	public float drag = 0.5f; //resistencia do solo
	public float terminalRotationSpeed = 25.0f;
	public VirtualJoystick moveJoystick;

	public float turboSpeed = 5.0f;
	public float turboCooldown = 2.0f;
	private float lastTurbo;

	private Rigidbody controller;
	private Transform camTransform;

	private void Start() {
		lastTurbo = Time.time - turboCooldown;

		controller = GetComponent<Rigidbody> ();
		controller.maxAngularVelocity = terminalRotationSpeed;
		controller.drag = drag;

		camTransform = Camera.main.transform;
	}

	private void Update() {
		Vector3 dir = Vector3.zero;

		//keyboard
		dir.x = Input.GetAxis ("Horizontal");
		dir.z = Input.GetAxis ("Vertical"); 

		if (dir.magnitude > 1)
			dir.Normalize ();

		//virtual joystick
		if (moveJoystick.InputDirection != Vector3.zero) {
			dir = moveJoystick.InputDirection;
		}

		// Rotate our direction vector (movement) with the camera
		Vector3 rotatedDir = camTransform.TransformDirection(dir);
		rotatedDir = new Vector3 (rotatedDir.x, 0, rotatedDir.z);
		rotatedDir = rotatedDir.normalized * dir.magnitude;

		controller.AddForce (rotatedDir * moveSpeed);
	}

	public void Turbo() {
		if (Time.time - lastTurbo > turboCooldown) {
			controller.AddForce (controller.velocity.normalized * turboSpeed, ForceMode.Impulse);
			lastTurbo = Time.time;
		}
	}
}
