using UnityEngine;
using System.Collections;
using System.Collections.Generic; // For List
using System.Linq; // Required for OrderBy and Select

public class CinematicIntroCamera : MonoBehaviour
{
    public Transform cameraRoutesParent;
    public int curveResolution = 20; // Resolution for the Catmull-Rom spline
    public float cameraMoveSpeed = 2f; // Speed at which the camera traverses the spline point
    public GameObject playerDrone;
    public GameObject droneCamera;

    private Vector3[] splinePathPoints; // Stores the pre-calculated points along the spline
    private float totalSplineLength;    // Total length of the generated spline path

    void Awake()
    {
        if (cameraRoutesParent == null)
        {
            Debug.LogError("Camera Routes Parent is not assigned to CinematicIntroCamera!", this);
            enabled = false;
            return;
        }

        List<Transform> controlPoints = new List<Transform>();
        for (int i = 0; i < cameraRoutesParent.childCount; i++)
        {
            controlPoints.Add(cameraRoutesParent.GetChild(i));
        }
        // Ensure consistent order, crucial for spline calculations
        controlPoints = controlPoints.OrderBy(t => t.name).ToList();

        if (controlPoints.Count < 2)
        {
            Debug.LogError("Not enough control points found for Catmull-Rom spline. Need at least 2.", this);
            enabled = false;
            return;
        }
        GenerateCatmullRomPath(controlPoints);

        if (splinePathPoints.Length == 0)
        {
            Debug.LogError("Failed to generate spline path points.", this);
            enabled = false;
            return;
        }


        transform.position = splinePathPoints[0];
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); 
        this.gameObject.SetActive(true);
    }

    void Start()
    {
        if (playerDrone != null) playerDrone.SetActive(false);
        if (droneCamera != null) droneCamera.SetActive(false);

        StartCoroutine(PlayCinematicIntro());
    }

    IEnumerator PlayCinematicIntro()
    {

        float currentPathTime = 0f; 

        while (currentPathTime < 1f)
        {
            float distanceToMove = cameraMoveSpeed * Time.deltaTime;
            currentPathTime += distanceToMove / totalSplineLength;

            currentPathTime = Mathf.Clamp01(currentPathTime);

            Vector3 newPosition = GetPositionOnSpline(currentPathTime);
            transform.position = newPosition;

            transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            yield return null;
        }

        Debug.Log("Cinematic intro finished. Switching to DroneCamera.");

        if (playerDrone != null)
        {
            playerDrone.SetActive(true);
        }

        if (droneCamera != null)
        {
            droneCamera.SetActive(true);
        }
        else
        {
            Debug.LogError("DroneCamera reference not set!");
        }

        this.gameObject.SetActive(false);

    }

    private void GenerateCatmullRomPath(List<Transform> controlPoints)
    {
        List<Vector3> generatedPoints = new List<Vector3>();
        totalSplineLength = 0f;

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Transform p0 = controlPoints[Mathf.Clamp(i - 1, 0, controlPoints.Count - 1)];
            Transform p1 = controlPoints[i];
            Transform p2 = controlPoints[i + 1];
            Transform p3 = controlPoints[Mathf.Clamp(i + 2, 0, controlPoints.Count - 1)];

            for (int j = 0; j < curveResolution; j++)
            {
                float t = j / (float)curveResolution;
                Vector3 position = CalculateCatmullRomPosition(t, p0.position, p1.position, p2.position, p3.position);
                generatedPoints.Add(position);
            }
        }

        generatedPoints.Add(controlPoints[controlPoints.Count - 1].position);

        splinePathPoints = generatedPoints.ToArray();

        for (int i = 0; i < splinePathPoints.Length - 1; i++)
        {
            totalSplineLength += Vector3.Distance(splinePathPoints[i], splinePathPoints[i + 1]);
        }
    }
    private Vector3 GetPositionOnSpline(float t)
    {
        if (splinePathPoints.Length == 0) return Vector3.zero;
        int index = Mathf.FloorToInt(t * (splinePathPoints.Length - 1));
        index = Mathf.Clamp(index, 0, splinePathPoints.Length - 2); 

        float segmentT = (t * (splinePathPoints.Length - 1)) - index; 

        return Vector3.Lerp(splinePathPoints[index], splinePathPoints[index + 1], segmentT);
    }

    private Vector3 CalculateCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return 0.5f * ((2f * p1) +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
    }
}
