using UnityEngine;
using TMPro; // Required for TextMeshPro

public class ShowAltitude : MonoBehaviour
{
    [Tooltip("Assign the TextMeshProUGUI component here to display the altitude.")]
    [SerializeField] private TextMeshProUGUI altitudeText;

    [Tooltip("The GameObject whose altitude you want to track. If null, this GameObject's altitude will be used.")]
    [SerializeField] private GameObject targetGameObject;

    [Tooltip("Offset for the altitude display (e.g., if ground is not at y=0).")]
    [SerializeField] private float altitudeOffset = 0f;
    private Rigidbody targetRigidbody;

    void Start()
    {
        // If altitudeText is not assigned, try to find one in the scene.
        // This is a fallback and it's better to assign it manually in the Inspector.
        if (altitudeText == null)
        {
            altitudeText = FindObjectOfType<TextMeshProUGUI>();
            if (altitudeText == null)
            {
                Debug.LogError("ShowAltitude: No TextMeshProUGUI component found in the scene. Please assign one in the Inspector or create a UI Text (TMP) object.");
                enabled = false; // Disable the script if no text component is found
                return;
            }
        }

        // If targetGameObject is not assigned, use the GameObject this script is attached to.
        if (targetGameObject == null)
        {
            targetGameObject = this.gameObject;
        }
        targetRigidbody = targetGameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (altitudeText != null && targetGameObject != null)
        {
            // Get the current Y position of the target GameObject
            float currentAltitude = targetGameObject.transform.position.y + altitudeOffset;
            float currentSpeed = 0f;
            if (targetRigidbody != null)
            {
                currentSpeed = targetRigidbody.velocity.magnitude; // Speed is the magnitude of velocity
            }

            // Update the TextMeshProUGUI component with the altitude
            // Using F2 to format to 2 decimal places for readability
            altitudeText.text = $"Altitude: {currentAltitude:F2}m \nSpeed: {currentSpeed:F2}m/s";
        }
    }
}
