using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodding : MonoBehaviour
{
    private Transform headDevice;
    public Transform XRRig; // The object to move (your XR rig)

    // Threshold values for detecting nodding gestures
    public float nodStartThreshold = 10.0f; // Rotation degree to start nodding
    public float nodEndThreshold = -10.0f;  // Rotation degree to end nodding

    private bool isNodding = false;

    void Start()
    {
        // Set the headDevice to the Main Camera's transform
        headDevice = GameObject.Find("MainCamera").transform;

        // Ensure the XRRig is set via Inspector or find it in the scene
        if (XRRig == null)
        {
            Debug.LogError("XRRig not assigned.");
        }
    }

    void Update()
    {
        if (headDevice != null && XRRig != null)
        {
            // Get the current rotation of the Main Camera
            Vector3 eulerRotation = headDevice.rotation.eulerAngles;

            // Convert rotation angles to a range from -180 to 180 for pitch (X-axis)
            float pitch = (eulerRotation.x > 180) ? eulerRotation.x - 360 : eulerRotation.x;

            // Check for nodding motion (moving the head up and down)
            if (!isNodding && pitch >= nodStartThreshold)
            {
                isNodding = true;
                Debug.Log("Nod Started");
            }
            else if (isNodding && pitch <= nodEndThreshold)
            {
                isNodding = false;
                Debug.Log("Nod Ended");

                // Move the XRRig forward by 0.5 units after a nod is completed
                MoveXRRigForward();
            }
        }
        else
        {
            Debug.LogError("Main Camera or XRRig not found");
        }
    }

    // Function to move the XRRig forward
    void MoveXRRigForward()
    {
        Vector3 forwardDirection = headDevice.forward; // Move in the direction the head (camera) is facing
        forwardDirection.y = 0; // Ignore vertical movement

        XRRig.position += forwardDirection.normalized * 0.5f; // Move forward by 0.5 units
        Debug.Log("Moved forward by 0.5 units");
    }
}
