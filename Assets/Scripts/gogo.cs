using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gogo : MonoBehaviour
{
    public Transform rightHandAvatar; // The right hand avatar (visual representation)
    public Transform rightController;  // The right hand controller (sensor input)
    public Transform chest;            // The chest or reference point (assign in the Inspector)

    public float thresholdDistance = 0.2f;  // Threshold distance for the Go-Go interaction
    public float scalingFactor = 3.0f;      // Scaling factor (k) for extending hand position

    void Update()
    {
        if (rightHandAvatar != null && rightController != null && chest != null)
        {
            // Get the current positions of the right hand avatar, controller, and chest
            Vector3 rightHandPosition = rightHandAvatar.position; // The visual representation
            Vector3 rightControllerPosition = rightController.position; // The sensor input
            Vector3 chestPosition = chest.position;

            // Calculate the distance between the chest and the right controller
            float distance = Vector3.Distance(chestPosition, rightControllerPosition);

            // If the controller distance exceeds the threshold, apply the Go-Go scaling to the right hand avatar
            if (distance > thresholdDistance)
            {
                // Calculate the direction vector from the chest to the right controller
                Vector3 direction = (rightControllerPosition - chestPosition).normalized;

                // Apply the Go-Go formula: new position = Rr + k * (distance - threshold)^2 * direction
                float extendedDistance = distance + scalingFactor * Mathf.Pow(distance - thresholdDistance, 2);

                // Update the right hand avatar position with the new extended position
                rightHandAvatar.position = chestPosition + direction * extendedDistance;
            }
            else
            {
                // Optionally, reset the position of the right hand avatar to match the right controller when within the threshold
                rightHandAvatar.position = rightControllerPosition;
            }
        }
        else
        {
            Debug.LogError("RightHandAvatar, RightController, or Chest not assigned in the Inspector.");
        }
    }
}