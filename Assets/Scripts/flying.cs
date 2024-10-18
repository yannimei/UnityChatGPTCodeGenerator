using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flying : MonoBehaviour
{
    private Transform headDevice; // The head (camera)
    public Transform XRRig;       // The object to move (your XR rig)
    public Transform leftHand;    // The left hand object (assign in the Inspector)
    public Transform rightHand;   // The right hand object (assign in the Inspector)

    public float flapThreshold = 0.2f;   // Minimum vertical movement to detect flapping
    public float flySpeed = 2.0f;        // Flying speed
    public float timeWindow = 0.2f;      // Time window to accumulate hand movements
    public float flyDuration = 5.0f;     // Optional: duration to stay in flying mode

    private bool isFlying = false;       // Whether the player is in flying mode
    private bool isFlapping = false;     // Whether a flapping motion has been detected

    private Vector3 initialLeftHandPosition;
    private Vector3 initialRightHandPosition;

    private float leftHandMovementAccumulated = 0.0f;
    private float rightHandMovementAccumulated = 0.0f;

    private float timeSinceLastFlap = 0.0f;
    private float flyingTime = 0.0f;     // Timer for how long player stays flying

    void Start()
    {
        // Set the headDevice to the Main Camera's transform
        headDevice = GameObject.Find("MainCamera").transform;

        // Ensure the XRRig and hands are set via Inspector or find them in the scene
        if (XRRig == null || leftHand == null || rightHand == null)
        {
            Debug.LogError("XRRig, leftHand, or rightHand not assigned.");
        }

        // Initialize initial hand positions
        initialLeftHandPosition = leftHand.localPosition;
        initialRightHandPosition = rightHand.localPosition;
    }

    void Update()
    {
        if (headDevice != null && XRRig != null && leftHand != null && rightHand != null)
        {
            // Accumulate hand movement over time
            timeSinceLastFlap += Time.deltaTime;

            // Detect hand flapping gestures within a time window
            AccumulateHandMovement();

            // If enough movement is accumulated in the time window, trigger flapping
            if (timeSinceLastFlap >= timeWindow)
            {
                DetectFlapping();
                timeSinceLastFlap = 0f; // Reset time window
                ResetAccumulatedMovement();
            }

            // If flying, move the XRRig upward, otherwise don't move
            if (isFlying)
            {
                Fly();
                flyingTime += Time.deltaTime;

                // Optionally, stop flying after a set duration
                if (flyingTime >= flyDuration)
                {
                    isFlying = false;  // Disable flying after the duration
                    Debug.Log("Flying Ended - Duration Expired");
                }
            }
            else
            {
                // Reset the flying time if not flying
                flyingTime = 0f;
            }
        }
        else
        {
            Debug.LogError("Main Camera, XRRig, or hands not found.");
        }
    }

    // Accumulate hand movement over time
    void AccumulateHandMovement()
    {
        float leftHandYMovement = leftHand.localPosition.y - initialLeftHandPosition.y;
        float rightHandYMovement = rightHand.localPosition.y - initialRightHandPosition.y;

        leftHandMovementAccumulated += leftHandYMovement;
        rightHandMovementAccumulated += rightHandYMovement;

        // Update initial positions for the next frame
        initialLeftHandPosition = leftHand.localPosition;
        initialRightHandPosition = rightHand.localPosition;
    }

    // Detect flapping gestures based on accumulated movement
    void DetectFlapping()
    {
        // Check if both hands have moved upwards significantly
        if (!isFlapping && leftHandMovementAccumulated >= flapThreshold && rightHandMovementAccumulated >= flapThreshold)
        {
            isFlapping = true;
            Debug.Log("Flapping Started");
        }
        // Check if both hands have moved down (flap completed)
        else if (isFlapping && leftHandMovementAccumulated <= -flapThreshold && rightHandMovementAccumulated <= -flapThreshold)
        {
            isFlapping = false;
            isFlying = true;  // Enable flying when the flap gesture is completed
            Debug.Log("Flapping Ended - Flying Enabled");
        }
    }

    // Reset accumulated hand movements
    void ResetAccumulatedMovement()
    {
        leftHandMovementAccumulated = 0.0f;
        rightHandMovementAccumulated = 0.0f;
    }

    // Move the XRRig upwards or forward when flying
    void Fly()
    {
        Vector3 upwardDirection = Vector3.up;  // Move upward by default

        // Optionally, you can make the player move forward in the direction the head is facing while flying
        Vector3 forwardDirection = headDevice.forward;
        forwardDirection.y = 0; // Ignore vertical component to prevent weird angles

        // Combine upward and forward movement for a flying effect
        Vector3 flyDirection = upwardDirection + forwardDirection.normalized;

        // Move the XRRig upwards or forward
        XRRig.position += flyDirection * flySpeed * Time.deltaTime;
        Debug.Log("Flying...");
    }
}
