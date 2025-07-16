using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHit : MonoBehaviour
{
    public string truckTag = "Target";
    public GameObject winMessagePanel;
    public GameObject loseMessagePanel;
    public GameObject gameUI;
    public GameObject drone; 
    public MonoBehaviour[] playerControlScripts; 
    private bool hasWon = false;
    public TruckMovement truckMovementScript;
    // Start is called before the first frame update
    void Awake()
    {
        if (winMessagePanel != null)
        {
            winMessagePanel.SetActive(false);
            loseMessagePanel.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(truckTag) && !hasWon && truckMovementScript.shouldMove)
        {
            hasWon = true; // Set win flag
            Debug.Log("Collision detected! Drone hit the truck. Player Wins!");

            // --- Implement Win Logic Here ---

            // 1. Show the win message
            if (winMessagePanel != null)
            {
                winMessagePanel.SetActive(true);
            }

            // 2. Hide in-game UI (optional)
            if (gameUI != null)
            {
                gameUI.SetActive(false);
            }

            if (drone != null)
            {
                drone.SetActive(false);
            }


            // 3. Stop all player controls
            foreach (MonoBehaviour script in playerControlScripts)
            {
                if (script != null) // Check if the reference is not null
                {
                    script.enabled = false;
                }
            }
            truckMovementScript.shouldMove = false;
        }
    }
}
