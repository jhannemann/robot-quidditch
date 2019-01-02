using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed;

    void Update()
    {
        // destroy object if it is on or below the ground
        if (transform.position.y <= transform.localScale.y/2.0f)
        {
            Destroy(gameObject);
        }
    }
}
