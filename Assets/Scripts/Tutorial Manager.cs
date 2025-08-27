using System;
using System.IO;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    // Controller input actions
    [SerializeField] public InputActionReference primaryButton;
    [SerializeField] public InputActionReference triggerButton;
    [SerializeField] public InputActionReference joystickUp;
    [SerializeField] public InputActionReference joystickDown;
    // Tutorial scene index
    // 0 > Start screen
    // 1 > Line pair example
    // 2 > Line pair without toggling
    // 3 > Line pair with toggling
    private enum sceneEnum
    {
        start,
        tutorial
    }
    private sceneEnum scene = sceneEnum.start;
    private GameObject sceneObj;
    private LinePair lp;
    private float[] UpDownTime = { 0, 0 };
    private bool[] UpDownHeld = { false, false };

    void Start()
    {
        // Create the line pair for future use
        lp = this.AddComponent<LinePair>();
        // Only bind the nextScene button
        primaryButton.action.performed += NextScene;
        // Show the initial info screen
        sceneObj = (GameObject)Instantiate(Resources.Load("Start Screen"));
    }

    public void NextScene(InputAction.CallbackContext context)
    {

        // Iterate to the next scene if possible
        if (scene == sceneEnum.tutorial)
        {
            // End of scenes, remove controls and continue to the line tester
            DeregisterControls();
            SceneManager.LoadScene(1);
            return;
        }

        // Destroy the existing scene
        Destroy(sceneObj);
        // Iterate the scene index
        scene += 1;
        // Set up the new scene
        buildTutorial();
    }
    private void buildTutorial()
    {
        // Set up scene and line pair
        sceneObj = (GameObject)Instantiate(Resources.Load("Tutorial Screen"));
        lp.MakeLines(0.5f);
        lp.lines.transform.position = new Vector3(10, 0, 0);
        // Register controls to change the line movement
        RegisterControls();
    }

    public void RegisterControls()
    {
        // Joystick scaling
        joystickUp.action.canceled += StopJUp;
        joystickUp.action.performed += StartJUp;
        joystickDown.action.canceled += StopJDown;
        joystickDown.action.performed += StartJDown;
    }

    private void StopJUp(InputAction.CallbackContext context)
    {
        UpDownHeld[0] = false;
        UpDownTime[0] = 0;
    }

    private void StartJUp(InputAction.CallbackContext context)
    {
        // Perform base action
        lp.IncreaseSize(triggerButton.action.inProgress);
        // Start adding to time
        UpDownHeld[0] = true;
    }

    private void StopJDown(InputAction.CallbackContext context)
    {
        UpDownHeld[1] = false;
        UpDownTime[1] = 0;
    }

    private void StartJDown(InputAction.CallbackContext context)
    {
        // Perform base action
        lp.DecreaseSize(triggerButton.action.inProgress);
        // Start adding to time
        UpDownHeld[1] = true;
    }

    void DeregisterControls()
    {
        // Remove NextScene button as well
        primaryButton.action.performed -= NextScene;
        joystickUp.action.canceled -= StopJUp;
        joystickUp.action.performed -= StartJUp;
        joystickDown.action.canceled -= StopJDown;
        joystickDown.action.performed -= StartJDown;
    }

    void Update()
    {
        if (UpDownHeld[0])
        {
            // Add delta time (done in seconds)
            UpDownTime[0] += Time.deltaTime;
            if (UpDownTime[0] > 0.5)
            {
                // Repeatedly increase size
                lp.IncreaseSize(true);
            }
        }
        // DOWN
        else if (UpDownHeld[1])
        {
            // Add delta time (done in seconds)
            UpDownTime[1] += Time.deltaTime;
            if (UpDownTime[1] > 0.5)
            {
                // Repeatedly increase size
                lp.DecreaseSize(true);
            }
        }
    }
}
