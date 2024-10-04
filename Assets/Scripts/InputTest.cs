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

    public void Update()
    {
        if (Input.GetButtonUp("XButton")) { Debug.LogWarning("x button pressed"); }
        if (Input.GetButtonUp("BButton")) { Debug.LogWarning("b button pressed"); }
        if (Input.GetButtonUp("AButton")) { Debug.LogWarning("a button pressed"); }
        if (Input.GetButtonUp("YButton")) { Debug.LogWarning("y button pressed"); }
    }
}