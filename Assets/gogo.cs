using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gogo : MonoBehaviour
{
    public Transform controller;  // Reference to the VR controller
    public Transform handModel;   // Hand visual representation in VR
    public float reachThreshold = 0.5f;  // Distance where GoGo scaling starts
    public float maxScaleFactor = 5.0f;  // Maximum stretch distance

    private Vector3 initialHandPosition;

    void Start()
    {
        // Store the initial hand position (relative to the user’s body or controller)
        initialHandPosition = controller.localPosition;
    }

    void Update()
    {
        // Calculate the real-world distance the hand has moved
        Vector3 currentHandPosition = controller.localPosition;
        float realDistance = Vector3.Distance(initialHandPosition, currentHandPosition);

        // If the hand is beyond the reach threshold, apply GoGo stretching
        if (realDistance > reachThreshold)
        {
            float scaleFactor = GetGoGoScale(realDistance);
            handModel.localPosition = initialHandPosition + (controller.localPosition - initialHandPosition) * scaleFactor;
        }
        else
        {
            // Keep the hand position normal when within threshold
            handModel.localPosition = controller.localPosition;
        }
    }

    // Function to calculate GoGo scaling factor based on distance
    float GetGoGoScale(float distance)
    {
        float stretchDistance = distance - reachThreshold;
        return Mathf.Lerp(1.0f, maxScaleFactor, stretchDistance / reachThreshold);
    }
}
