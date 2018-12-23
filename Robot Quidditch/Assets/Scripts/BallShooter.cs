using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
	public GameObject ball;

	private float speed;
	private Rigidbody rb;

	
	// Use this for initialization
	void Start ()
	{
		speed = ball.GetComponent<Ball>().speed;
		rb = ball.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update()
	{
		// Ctrl was pressed, launch a projectile
		if (Input.GetButtonDown("Fire1"))
		{
			Transform t = transform;
			// Instantiate the projectile at the position of the barrel and translate along the barrel by its length
			var length = t.localScale.y;
			var direction = t.TransformDirection(Vector3.up);
			var clone = Instantiate<Rigidbody>(rb, 
				t.position + length * direction,
				Quaternion.identity);
			clone.velocity = speed * direction;
		}
	}
}
