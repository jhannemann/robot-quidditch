// SPDX-License-Identifier: GPL-3.0-or-later

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	private BallShooter shooter;
	
	// state and event variables to determine next action
	private enum State
	{
		Stopped,
		MovingForward,
		MovingBackward
	};

	public enum Event
	{
		LockAcquired,
		LockLost,
		FrontSwitchOn,
		BackSwitchOn
	}
	
	private bool previousHit;
	private bool currentHit;
	private bool locked;
	private float timeOfLockAcquired;
	private float timeOfLockLost;
	private State state;
	private bool[] events;
	
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

	void Start()
	{
		shooter = GetComponentInChildren<BallShooter>();
		steering = 0.0f;
		motor = 0.0f;
		previousHit = false;
		currentHit = false;
		locked = false;
		timeOfLockLost = Time.fixedTime;
		timeOfLockAcquired = Time.fixedTime;
		events = new bool[4];
		state = State.MovingForward;
		MoveForward();
	}

	private void CastRay()
	{
		previousHit = currentHit;

		Vector3 dir = transform.TransformDirection(Vector3.right);		
		RaycastHit hit;
		// Does the ray intersect any objects excluding the player layer
		if (Physics.Raycast(transform.position, dir, out hit))
		{
			Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
			currentHit = true;
		}
		else
		{
			Debug.DrawRay(transform.position, dir * 1000, Color.red);
			currentHit = false;
		}
	}

	public void ClearEvent(Event ev)
	{
		Assert.IsNotNull(events);
		events[(int) ev] = false;
	}

	public void SetEvent(Event ev)
	{
		events[(int) ev] = true;
	}

	public bool GetEvent(Event ev)
	{
		return events[(int) ev];
	}
	
	private void UpdateLockStatus()
	{
		if (currentHit && previousHit)
		{
			Assert.IsTrue(locked);
		}
		else if (currentHit && !previousHit)
		{
			Assert.IsFalse(locked);
			locked = true;
			timeOfLockAcquired = Time.fixedTime;
			SetEvent(Event.LockAcquired);
		}
		else if(!currentHit && previousHit){
			Assert.IsTrue(locked);
			locked = false;
			timeOfLockLost = Time.fixedTime;
			SetEvent(Event.LockLost);
		}
		else
		{
			Assert.IsFalse(locked);
		}
	}

	private void MoveBackward()
	{
		motor = -1.0f;
	}

	private void MoveForward()
	{
		motor = 1.0f;
	}

	private void Stop()
	{
		motor = 0.0f;
	}

	private void AutonomousMove()
	{
		Assert.IsTrue(autonomous);
		CastRay();
		UpdateLockStatus();

		if (GetEvent(Event.LockAcquired))
		{
			state = State.Stopped;
			Stop();
			ClearEvent(Event.LockAcquired);
		}
		
		if (GetEvent(Event.LockLost))
		{
			state = State.MovingForward;
			MoveForward();
			ClearEvent(Event.LockLost);
		}
		
		if (GetEvent(Event.FrontSwitchOn))
		{
			state = State.MovingBackward;
			MoveBackward();
			ClearEvent(Event.FrontSwitchOn);
		}

		if (GetEvent(Event.BackSwitchOn))
		{
			state = State.MovingForward;
			MoveForward();
			ClearEvent(Event.BackSwitchOn);
		}
	}

	private void ManualMove()
	{
		motor = maxMotorTorque * Input.GetAxis("Vertical");
		steering = maxSteeringAngle * Input.GetAxis("Horizontal");
	}

	void FixedUpdate()
	{
		if (!Game.instance.started) return;
		if (autonomous)
		{
			AutonomousMove();
		}
		else
		{
			ManualMove();
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

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Loading Area"))
		{
			shooter.reload();
		}
	}
}
