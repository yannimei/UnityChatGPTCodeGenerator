using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/* How to align the virtual scene with the physical environment:
At the start of the app, place the controller where the reference point A should be,
press button 1, place the controller where the reference point B should be, press button 1.
The scene should be aligned. Press button 1 again to stop the alignment and hide the reference 
points, or button 2 to start again. */


public class Alignment : MonoBehaviour
{
    /* Attach this script to an empty gameObject that is the parent of all the objects that
    should be impacted by the alignment, including the reference points.
    The XR camera / controllers shouldn't be a children of that object, as it's position doesnt
    depend on the alignment.*/

    /* Create two gameObjects that will be used as reference points (eg. a small sphere for point A and
    a small cube for point B), and place them in the scene, at a position that is easy to locate in 
    the physical environment (eg. corners of the room or corners of a table)*/
    [SerializeField]
    private GameObject referencePointA;

    [SerializeField]
    private GameObject referencePointB;

    [SerializeField]
    private GameObject referencePointAProxy;

    [SerializeField]
    private GameObject referencePointBProxy;

    // Positions of the reference points
    private Vector3 virtualPositionA;
    private Vector3 virtualPositionB;

    // Positions where the reference points should be
    private Vector3 realPositionA;
    private Vector3 realPositionB;

    // Progress of the alignment
    private int alignmentState = 0;

    [SerializeField]
    private GameObject visualisationModel;

    [SerializeField]
    private Transform calibrationPointer;

    [SerializeField]
    private Transform fineTuningParent;

    [SerializeField] 
    private TextMeshProUGUI modeText; // UI text to show mode

    [SerializeField]
    private float scalingFactor = 0.01f;

    private enum FineTuneMode { Translation, Rotation, ResetUp }
    private FineTuneMode currentMode = FineTuneMode.Translation;
    private bool endFineTuning = false;

    // Start is called before the first frame update
    void Start()
    {
        virtualPositionA = referencePointA.transform.position; 
        virtualPositionB = referencePointB.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //OVRInput.Update();

        if (alignmentState == -1)
        {
            alignmentState = 0;
        }

        if (alignmentState == 2)
        {
            // The alignment is done.
            // When button 1 pressed, stop and hide reference points
            if (OVRInput.GetDown(OVRInput.Button.One)) {
                referencePointA.SetActive(false);
                referencePointB.SetActive(false);
                referencePointAProxy.SetActive(false);
                referencePointBProxy.SetActive(false);

                alignmentState = 3;
                //set the model visible
                visualisationModel.SetActive(true);
            }

            // When button 2 pressed, start again
            if (OVRInput.GetDown(OVRInput.Button.Two)) {
                virtualPositionA = referencePointA.transform.position;
                virtualPositionB = referencePointB.transform.position;

                alignmentState = -1;
            }
        }

        if (alignmentState == 1)
        {
            // When button 1 pressed again, get the position where the reference point B should be
            if (OVRInput.GetDown(OVRInput.Button.One)) {

                realPositionB = calibrationPointer.position;
                Debug.Log("Point B set");
                referencePointBProxy.transform.position = realPositionB;
                // A little bit of geometry to rotate, translate and rescale the parent of the objects,
                // to make the reference points match. 
                //Note that the rotation should always be done before the translation and rescaling.

                // Rotate
                Quaternion rotationOffset = Quaternion.FromToRotation(virtualPositionB - virtualPositionA, realPositionB - realPositionA);
                transform.rotation = rotationOffset * transform.rotation;
                virtualPositionA = referencePointA.transform.position;
                virtualPositionB = referencePointB.transform.position;

                // Rescale 
                // float scaleFactor = (realPositionB - realPositionA).magnitude / (virtualPositionB - virtualPositionA).magnitude;
                // transform.localScale = transform.localScale * scaleFactor;
                // virtualPositionA = referencePointA.transform.position;
                // virtualPositionB = referencePointB.transform.position;

                // Translate 
                transform.position = transform.position + (realPositionA - virtualPositionA);

                Debug.Log("Alignment done, push button 1 again to confirm, button 2 to restart");

                alignmentState = 2;
            }
        }


        if (alignmentState==0){
            // Start of the alignment
            // When button 1 pressed, get the position where the reference point A should be
            if (OVRInput.GetDown(OVRInput.Button.One)) {

                realPositionA = calibrationPointer.position;
                Debug.Log("Point A set");
                referencePointAProxy.transform.position = realPositionA;
                alignmentState = 1;
            }
        }

        //if alignment done start finetuning
        if (AlignmentDone())
        {
            UpdateModeText();

            if (OVRInput.GetDown(OVRInput.Button.Three)) // X Button on Oculus
            {
                CycleFineTuneMode();
            }

            if (OVRInput.GetDown(OVRInput.Button.Four)) //if press Y button, end fine-tuning and enable interact with the mo
            {
                endFineTuning = true;
                EnableModelInteraction();
            }


            //Vector2 leftJoystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) * scalingFactor * Time.deltaTime;
            //Vector2 rightJoystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) * scalingFactor * Time.deltaTime;

            switch (currentMode)
            {
                case FineTuneMode.Translation:
                    Vector2 leftJoystickT = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) * scalingFactor * Time.deltaTime;
                    Vector2 rightJoystickT = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) * scalingFactor * Time.deltaTime;
                    FineTuneTranslation(leftJoystickT, rightJoystickT);
                    break;
                case FineTuneMode.Rotation:
                    Vector2 leftJoystickR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) * scalingFactor ;
                    Vector2 rightJoystickR = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) * scalingFactor;
                    FineTuneRotation(leftJoystickR, rightJoystickR);
                    break;
                case FineTuneMode.ResetUp:
                    FineTuneResetUpAxis();
                    break;
            }
        }

    }

    public bool AlignmentDone()
    {
        return alignmentState == 3;
    }

    // fine tune the alignment
    public void FineTuneTranslation(Vector2 leftJoystick, Vector2 rightJoystick)
    {
        // set flag to make sure the new positions after fine tuning are saved with new spatial anchors (TBD when SA integrated)
        //this.newPositions = true;

        // map joystick inputs to a translation vector
        float x = leftJoystick.x;
        float y = rightJoystick.y;
        float z = leftJoystick.y;

        // apply the translation
        this.fineTuningParent.Translate(x, y, z, Space.World);
    }

    // fine tune the alignment
    public void FineTuneRotation(Vector2 leftJoystick, Vector2 rightJoystick)
    {
        // set flag to make sure the new positions after fine tuning are saved with new spatial anchors
        //this.newPositions = true;

        // map joystick inputs to a rotation vector
        float x = leftJoystick.y;
        float y = rightJoystick.x;
        float z = leftJoystick.x;

        // apply the rotation (only rotates around a single axis by choosing the one with the highest absolute input)
        if (Mathf.Abs(x) >= Mathf.Abs(y) && Mathf.Abs(x) >= Mathf.Abs(z)) this.fineTuningParent.Rotate(x, 0, 0, Space.Self);
        else if (Mathf.Abs(y) >= Mathf.Abs(z)) this.fineTuningParent.Rotate(0, y, 0, Space.Self);
        else this.fineTuningParent.Rotate(0, 0, z, Space.Self);
    }

    // reset the up-axis of the model to the global up-axis
    public void FineTuneResetUpAxis()
    {
        Quaternion rotationOffset = Quaternion.FromToRotation(this.fineTuningParent.up, Vector3.up);
        this.fineTuningParent.rotation = rotationOffset * this.fineTuningParent.rotation;
    }

    private void CycleFineTuneMode()
    {
        if (currentMode == FineTuneMode.Translation)
            currentMode = FineTuneMode.Rotation;
        else if (currentMode == FineTuneMode.Rotation)
            currentMode = FineTuneMode.ResetUp;
        else
            currentMode = FineTuneMode.Translation;

        UpdateModeText();
    }

    private void UpdateModeText()
    {
        if (modeText != null)
        {
            modeText.text = "Mode: " + currentMode.ToString();
            if (endFineTuning) { modeText.text = "Fine-tuning complete!"; }
        }


    }


    private void EnableModelInteraction()
    {
        var placeSkyScraper = FindObjectOfType<PlaceSkyscraper>();
        placeSkyScraper.isReady = true;
    }
}
