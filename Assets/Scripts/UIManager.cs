using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiobject;
    private Vector3 offset = new Vector3 (0, 0.1f, 1.2f);

    public void Update()
    {
        if (Input.GetButtonUp("LeftMenuButton"))
        {
            AdaptUI();
        }
    }

    public void AdaptUI()
    {
        GameObject camera = GameObject.Find("MainCamera");

        // Step 1: Move the UI in front of the camera using the offset
        uiobject.transform.position = camera.transform.position + camera.transform.TransformDirection(offset);

        // Step 2: Make the UI face towards the camera
        // Calculate the direction from the UI to the camera
        Vector3 directionToCamera = camera.transform.position - uiobject.transform.position;
        // Create a rotation that looks in that direction
        Quaternion rotationToCamera = Quaternion.LookRotation(-directionToCamera);
        // Apply the rotation to the UI, setting it to face the camera
        uiobject.transform.rotation = Quaternion.Euler(0, rotationToCamera.eulerAngles.y, 0); // Only rotate around Y-axis

    }
}
