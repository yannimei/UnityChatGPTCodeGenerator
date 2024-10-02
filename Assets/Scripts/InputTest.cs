using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    private GameObject apple;
    private GameObject apple2;
    private Transform rightHand;
    private Vector3 initialOffset;
    private Quaternion initialRotationDifference;

    void Start()
    {
        // Find the original Apple
        apple = GameObject.Find("Apple");

        if (apple == null)
        {
            Debug.LogError("Original Apple not found!");
            return;
        }

        // Find the right hand
        rightHand = GameObject.Find("XRRig/CameraOffset/RightController/RightHand").transform;

        if (rightHand == null)
        {
            Debug.LogError("Right hand not found!");
            return;
        }

        // Create Apple2 as a duplicate of Apple
        apple2 = Instantiate(apple, rightHand.position, apple.transform.rotation);
        apple2.name = "Apple2";

        // Calculate initial offset between Apple and Apple2
        initialOffset = apple.transform.position - apple2.transform.position;

        // Calculate initial rotation difference
        initialRotationDifference = Quaternion.Inverse(apple2.transform.rotation) * apple.transform.rotation;

        // Set Apple2's parent to the right hand
        apple2.transform.SetParent(rightHand);
    }

    void Update()
    {
        if (apple == null || apple2 == null)
            return;

        // Calculate the movement of Apple2 relative to its initial position
        Vector3 apple2Movement = apple2.transform.position - rightHand.position;

        // Apply the inverse of this movement to Apple, maintaining the initial offset
        apple.transform.position = apple2.transform.position + initialOffset - apple2Movement;

        // Calculate the rotation of Apple2 relative to its initial rotation
        Quaternion apple2Rotation = Quaternion.Inverse(rightHand.rotation) * apple2.transform.rotation;

        // Apply the inverse of this rotation to Apple, maintaining the initial rotation difference
        apple.transform.rotation = Quaternion.Inverse(apple2Rotation) * initialRotationDifference * apple2.transform.rotation;
    }
}