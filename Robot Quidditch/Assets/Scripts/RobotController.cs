using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
}
     
public class RobotController : MonoBehaviour {
	public List<AxleInfo> axleInfos; 
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public bool autonomous;

	// movement parameters, either set by GetAxis() or AutonomousMove()
	private float motor;
	private float steering;
     
	// finds the corresponding visual wheel
	// correctly applies the transform
	private void ApplyLocalPositionToVisuals(WheelCollider collider)
	{
		if (collider.transform.childCount == 0) {
			return;
		}
     
		Transform visualWheel = collider.transform.GetChild(0);
     
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
     
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}


	private void Start()
	{
		steering = 0.0f;
		motor = 0.0f;
	}

	private void AutonomousMove()
	{
		Assert.IsTrue(autonomous);
	}

	public void FixedUpdate()
	{
		if (autonomous)
		{
			AutonomousMove();
		}
		else
		{
			motor = maxMotorTorque * Input.GetAxis("Vertical");
			steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		}

		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if (axleInfo.motor) {
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			}
			ApplyLocalPositionToVisuals(axleInfo.leftWheel);
			ApplyLocalPositionToVisuals(axleInfo.rightWheel);
		}
	}
}