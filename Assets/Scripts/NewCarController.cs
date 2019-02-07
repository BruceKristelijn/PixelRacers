using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCarController : MonoBehaviour {

	public WheelCollider frontLeftWheel;
	public WheelCollider frontRightWheel;
	public WheelCollider backLeftWheel;
	public WheelCollider backRightWheel;

	public float maxMotorPower = 100;
	public float maxSteerAngle = 30;

	void FixedUpdate() {
		float motor = maxMotorPower * -Input.GetAxis ("Vertical");
		float steering = maxSteerAngle * Input.GetAxis("Horizontal");

		frontLeftWheel.steerAngle = steering;
		frontRightWheel.steerAngle = steering;

		backLeftWheel.motorTorque = motor;
		backRightWheel.motorTorque = motor;

		//Wheel Turning
		TurnWheel(frontLeftWheel);
		TurnWheel(frontRightWheel);
		TurnWheel(backLeftWheel);
		TurnWheel(backRightWheel);

		var rotationVector = transform.rotation.eulerAngles;
		rotationVector.z = 0;
		transform.rotation = Quaternion.Euler(rotationVector);
	}

	void TurnWheel(WheelCollider collider) {
		Transform wheel = collider.transform.GetChild (0);
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose (out position, out rotation);

		wheel.transform.position = position;
		wheel.transform.rotation = rotation;
	}


}
