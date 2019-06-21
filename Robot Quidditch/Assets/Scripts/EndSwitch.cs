// SPDX-License-Identifier: GPL-3.0-or-later

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSwitch : MonoBehaviour
{
    public RobotController controller;
   
    void OnCollisionStay()
    {
        if (gameObject.name == "Front Switch")
        {
            controller.SetEvent(RobotController.Event.FrontSwitchOn);
        }
        if (gameObject.name == "Back Switch")
        {
            controller.SetEvent(RobotController.Event.BackSwitchOn);
        }
    }
}
