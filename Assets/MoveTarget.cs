using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    public GameObject gateFrame;
    private bool hasBeenPassed = false;
    public string droneTag = "Player";
    public TruckMovement truckToMove;

    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the drone and if it hasn't been triggered yet
        if (other.CompareTag(droneTag) && !hasBeenPassed)
        {
            if (truckToMove != null)
            {
                truckToMove.StartMoving(); // Tell the truck to start moving
                hasBeenPassed = true; // Mark as triggered
                Debug.Log(gameObject.name + ": Drone passed! Triggering truck movement.");
            }
            else
            {
                Debug.LogWarning("Truck to Move is not assigned on " + gameObject.name + " GateTrigger script!");
            }
        }
    }
}
