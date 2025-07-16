using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicIntro : MonoBehaviour
{
    public Transform cameraRoutesParent;
    private Transform[] pathWaypoints; 
    public float cameraMoveSpeed = 2f;
    public float transitionToPlayerTime = 1f;
    public GameObject CinematicIntroCamera;
    public GameObject DiscoCustom;
    public GameObject DroneCamera;
    private int currentWaypointIndex = 0;
    private bool introFinished = false;

    void Awake() // Use Awake to ensure waypoints are gathered before Start
    {
        pathWaypoints = new Transform[cameraRoutesParent.childCount];
        for (int i = 0; i < cameraRoutesParent.childCount; i++)
        {
            pathWaypoints[i] = cameraRoutesParent.GetChild(i);
        }
        transform.position = pathWaypoints[0].position;
    }
    void Start()
    {
        if (DiscoCustom != null) DiscoCustom.SetActive(false);
        if (DroneCamera != null) DroneCamera.SetActive(false);
        StartCoroutine(PlayCinematicIntro());
    }

    IEnumerator PlayCinematicIntro()
    {
        // Disable any player input scripts on the drone initially
        int currentWaypointIndex = 0;
        // Move camera along path
        while (currentWaypointIndex < pathWaypoints.Length)
        {
            Transform targetWaypoint = pathWaypoints[currentWaypointIndex];
            Vector3 startPosition = transform.position;

            float journeyLength = Vector3.Distance(startPosition, targetWaypoint.position);
            float startTime = Time.time;

            while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f)
            {
                float distCovered = (Time.time - startTime) * cameraMoveSpeed;
                float fractionOfJourney = journeyLength > 0 ? distCovered / journeyLength : 1f;

                transform.position = Vector3.Lerp(startPosition, targetWaypoint.position, fractionOfJourney);
                transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                yield return null;
            }
            currentWaypointIndex++;
            yield return new WaitForSeconds(0.1f);
        }

        introFinished = true;
        Debug.Log("Cinematic intro finished. Transitioning to player control.");
        DiscoCustom.SetActive(true);
        DroneCamera.SetActive(true);
        this.gameObject.SetActive(false);
    }
    // Update is called once per frame
        void Update()
    {
        
    }
}
