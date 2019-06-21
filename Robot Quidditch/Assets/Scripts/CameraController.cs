// SPDX-License-Identifier: GPL-3.0-or-later

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objectToFollow;

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.LookAt(objectToFollow);
    }
}
