using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        // destroy object if it is on or below the ground
        if (transform.position.y <= transform.localScale.y/2.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Target"))
        {
            Game.instance.score += 1;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Blocker"))
        {
            Game.instance.shotsBlocked += 1;
        }    }
}
