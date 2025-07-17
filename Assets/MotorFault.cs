using System.Collections;
using System.Collections.Generic;
using PA_DronePack;
using UnityEngine;

public class MotorFault : MonoBehaviour
{
    public GameObject gateFrame;
    private bool hasBeenPassed = false;
    public string droneTag = "Player";
    public GameObject drone;
    public DroneControllerEx motorToFault;
    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the drone and if it hasn't been triggered yet
        if (other.CompareTag(droneTag) && !hasBeenPassed)
        {
            if (motorToFault != null)
            {
                motorToFault.MotorFault(); // Tell the truck to start moving
                hasBeenPassed = true; // Mark as triggered
                Debug.Log(gameObject.name + ": Drone passed! Motor Fault.");
            }
            else
            {
                Debug.LogWarning("Motor fault is not assigned on " + gameObject.name + " GateTrigger script!");
            }
        }
    }
}
