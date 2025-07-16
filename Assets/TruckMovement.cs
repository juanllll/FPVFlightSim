using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckMovement : MonoBehaviour
{
    public Transform targetPosition;
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;

    [Header("Audio Settings")]
    public AudioClip engineLoopSound;    // Assign your looping engine sound here
    private AudioSource audioSource;     // Reference to the AudioSource component

    public bool shouldMove = false;
    private bool isMovingSoundPlaying = false; // To prevent starting loop multiple times
    public GameObject loseMessagePanel;
    public GameObject drone; 

    void Awake() // Use Awake to get component reference
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("TruckMovement: AudioSource component not found on this GameObject.", this);
        }
    }

    void Update()
    {
        if (shouldMove)
        {
            // Start engine sound if not already playing
            if (audioSource != null && engineLoopSound != null && !isMovingSoundPlaying)
            {
                audioSource.clip = engineLoopSound;
                audioSource.loop = true; // Ensure the sound loops
                audioSource.Play();
                isMovingSoundPlaying = true;
            }

            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

            // Check if the truck has reached the target
            if (Vector3.Distance(transform.position, targetPosition.position) < stopDistance)
            {
                shouldMove = false; // Stop moving
                Debug.Log("Truck reached its destination!");
                loseScreen();
            }
        }
    }

    public void StartMoving()
    {
        shouldMove = true;
        Debug.Log("Truck started moving!");
    }

    public void loseScreen()
    {
        loseMessagePanel.SetActive(true);
         if (drone != null)
        {
            drone.SetActive(false);
        }
    }

}