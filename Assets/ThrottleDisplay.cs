using System.Collections;
using System.Collections.Generic;
using PA_DronePack;
using UnityEngine;

public class ThrottleDisplay : MonoBehaviour
{
    public RectTransform throttleIndicator; // Assign your "ThrottleIndicator" RectTransform here
    public RectTransform throttleContainer; // Assign your "ThrottleContainer" RectTransform here (the background)
    public DroneControllerEx droneController; // Replace DroneFlightController with your actual script name

    private float containerUsableHeight;
    private float indicatorHalfHeight;

    void Start()
    {
        if (droneController == null || throttleIndicator == null || throttleContainer == null)
        {
            Debug.LogError("ThrottleDisplayFixedBar: Missing references! Please assign droneController, throttleIndicator, and throttleContainer in the Inspector.");
            enabled = false;
            return;
        }

        containerUsableHeight = throttleContainer.rect.height;
        indicatorHalfHeight = throttleIndicator.rect.height / 2f;
    }

    void Update()
    {
        if (droneController == null) return;

        float currentThrottle = droneController.throttleForce;
        float maxAllowedThrottle = droneController.maxThrottleForce;

        currentThrottle = Mathf.Clamp(currentThrottle, 0f, maxAllowedThrottle);

        float normalizedThrottle = currentThrottle / maxAllowedThrottle;

        float minIndicatorCenterY = -(containerUsableHeight / 2f) + indicatorHalfHeight;
        float maxIndicatorCenterY = (containerUsableHeight / 2f) - indicatorHalfHeight;

        //Debug.Log("Min Indicator Center Y: " + minIndicatorCenterY);
        //Debug.Log("Max Indicator Center Y: " + maxIndicatorCenterY);

        float finalYPosition = Mathf.Lerp(minIndicatorCenterY, maxIndicatorCenterY, normalizedThrottle);

        throttleIndicator.anchoredPosition = new Vector2(throttleIndicator.anchoredPosition.x, finalYPosition);
    }
}

