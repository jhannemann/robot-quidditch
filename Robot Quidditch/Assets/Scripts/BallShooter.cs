using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
	public List<GameObject> balls;

	private int currentBallIndex;
	private GameObject currentBall;
	private float speed;
	private Rigidbody rb;
	private int numberOfBalls;

	private void switchBall()
	{
		currentBallIndex = (currentBallIndex+1)%balls.Count;
		currentBall = balls[currentBallIndex];
		speed = currentBall.GetComponent<Ball>().speed;
		rb = currentBall.GetComponent<Rigidbody>();
	}

	public void reload()
	{
		numberOfBalls = Game.instance.maxBalls;
	}
	
	// Use this for initialization
	void Start ()
	{
		currentBallIndex = balls.Count;
		switchBall();
		reload();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!Game.instance.started) return;
		
		// Ctrl was pressed, launch a projectile
		if (Input.GetButtonDown("Fire1") && numberOfBalls>0)
		{
			numberOfBalls--;
			Transform t = transform;
			// Instantiate the projectile at the position of the barrel and translate along the barrel by its length
			var length = t.localScale.y;
			var direction = t.TransformDirection(Vector3.up);
			var clone = Instantiate<Rigidbody>(rb, 
				t.position + length * direction,
				Quaternion.identity);
			clone.velocity = speed * direction;
		}

		if (Input.GetButtonDown("Fire3"))
		{
			switchBall();
		}
	}
}
