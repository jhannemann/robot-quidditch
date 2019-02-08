using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
	public GameObject currentBall;
	private float speed;
	private Rigidbody rb;

	private void Start()
	{
		speed = currentBall.GetComponent<Ball>().speed;
		rb = currentBall.GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			Transform t = transform;
			// Instantiate the projectile at the position of the barrel and translate along the barrel by its length
			var length = t.localScale.y;
			var direction = t.TransformDirection(Vector3.up);
			var clone = Instantiate(rb, t.position + length * direction, Quaternion.identity);
			clone.velocity = speed * direction;
		}
	}
}
