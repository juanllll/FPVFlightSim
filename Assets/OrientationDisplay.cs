using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationDisplay : MonoBehaviour
{
    public Transform droneTransform; // Drag your drone's GameObject here
    public RectTransform indicatorRectTransform; // Drag your "AttitudeIndicator" Image's RectTransform here
    public RectTransform displayBackgroundRectTransform; // Drag this GameObject's RectTransform here (the circular background)
    [Range(1, 90)] // Max angle in degrees that corresponds to the edge of the circle
    public float maxDisplayAngle = 45f;

    private float displayRadius;

    void Start()
    {
        if (droneTransform == null || indicatorRectTransform == null || displayBackgroundRectTransform == null)
        {
            Debug.LogError("AttitudeDisplay: Missing references! Please assign droneTransform, indicatorRectTransform, and displayBackgroundRectTransform in the Inspector.");
            enabled = false; // Disable script if references are missing
            return;
        }

        // Calculate the effective radius of the display circle
        // We use the smaller dimension (width or height) to ensure the indicator stays within a perfect circle.
        displayRadius = Mathf.Min(displayBackgroundRectTransform.rect.width, displayBackgroundRectTransform.rect.height) / 2f;
    }

    void Update()
    {
        if (droneTransform == null) return;
        Quaternion currentRotation = droneTransform.rotation;

        // Get forward vector in world space
        Vector3 forward = currentRotation * Vector3.forward;
        // Get up vector in world space
        Vector3 up = currentRotation * Vector3.up;

        Vector3 horizontalForward = new Vector3(forward.x, 0, forward.z).normalized;
        float pitchAngle = Vector3.SignedAngle(horizontalForward, forward, Vector3.right); // Pitch around local X-axis
        Vector3 eulerAngles = currentRotation.eulerAngles;

        float roll = NormalizeAngle(eulerAngles.z); // Z-axis rotation is roll
        float pitch = NormalizeAngle(eulerAngles.x); // X-axis rotation is pitch

        pitch = Mathf.Clamp(pitch, -maxDisplayAngle, maxDisplayAngle);
        roll = Mathf.Clamp(roll, -maxDisplayAngle, maxDisplayAngle);

        // Calculate normalized positions (-1 to 1) within the display
        float normalizedX = roll / maxDisplayAngle; // -1 to 1 based on roll
        float normalizedY = -pitch / maxDisplayAngle; // -1 to 1 based on pitch (inverted for UI: positive pitch (nose up) moves dot down)

        // Convert normalized positions to pixel positions relative to the center of the background
        float posX = -normalizedX * displayRadius;
        float posY = -normalizedY * displayRadius;

        // Set the indicator's local position
        indicatorRectTransform.anchoredPosition = new Vector2(posX, posY);
    }
    float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }
}
