using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LineTestManager : MonoBehaviour
{
    // Camera objects
    [SerializeField] public GameObject staticCamera;
    [SerializeField] public GameObject xrOrigin;
    [SerializeField] public Transform xrCamera;
    // Controller input actions
    [SerializeField] public InputActionReference primaryButton;
    [SerializeField] public InputActionReference triggerButton;
    [SerializeField] public InputActionReference joystickUp;
    [SerializeField] public InputActionReference joystickDown;
    [SerializeField] public bool logData = false;
    // Scenes, in order
    private enum sceneEnum
    {
        scene_user,
        scene_static,
        lp_horizontal,
        head_horizontal,
        lp_vertical,
        head_vertical,
        lp_diagonal,
        scene_dynamic,
        lp_dy_horizontal,
        lp_dy_vertical,
        lp_dy_diagonal,
        scene_end

    }
    private sceneEnum scene = 0;
    // Tools for current scene management
    private GameObject sceneObj;
    private LinePair lp;
    private TextMeshPro instructionText;
    private TextMeshPro eyeText;
    private int eyeVal = 20;
    // Tools for line pair scaling
    private float[] UpDownTime = { 0, 0 };
    private bool[] UpDownHeld = {false, false};
    // Data for screenshotting and file writing
    private string UUID = System.Guid.NewGuid().ToString();
    // Update the file and directory paths to accomodate the current application path
    // Can't use Path.Combine() since it gets funky with the slashes
    private string dirPath = "VRRT Data";
    private string filePath = "VRRTData.csv";

    void Start()
    {
        Debug.Log(Screen.currentResolution);
        // Set up a line pair tool
        lp = this.AddComponent<LinePair>();
        lp.SetCamera(xrCamera);
        // Set up data logging if toggled
        if (logData)
        {
            // Set up proper file paths
            dirPath = Application.persistentDataPath + "/" + dirPath;
            filePath = dirPath + "/" + filePath;

            // Init logging with the line pair tool
            lp.InitLog(dirPath, filePath, UUID);
        }
        RegisterControls();
        // Show the user initialization screen
        sceneObj = (GameObject)Instantiate(Resources.Load("User Init Screen"));
        // Set up the user init screen
        SetUpUserInitMenu();
        // Also instantiate the instruction text
        var textObject = new GameObject();
        textObject.name = "Instruction Text";
        textObject.AddComponent<TextMeshPro>();
        textObject.transform.Rotate(90, 0, 0);
        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 5);
        instructionText = textObject.GetComponent<TextMeshPro>();
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.fontSize = 20;
    }

    public void SetUpUserInitMenu()
    {
        // Set text to UUID
        var uuidText = GameObject.Find("UUID").GetComponent<TextMeshPro>().text = UUID;
        eyeText = GameObject.Find("Eye Score").GetComponent<TextMeshPro>();
        // Set up basic controls
        RegisterInitControls();
        // Set eye value
        eyeText.text = "20/" + eyeVal;
        
    }

    public void RegisterInitControls()
    {
        // Set up action bindings
        primaryButton.action.performed += NextScene;
        // Change prescription numbers with joysticks
        joystickUp.action.performed += EyeUp;
        joystickDown.action.performed += EyeDown;    
    }

    public void DeregisterInitControls()
    {
        joystickUp.action.performed -= EyeUp;
        joystickDown.action.performed -= EyeDown;  
    }

    public void RegisterControls()
    {
        // Joystick scaling
        joystickUp.action.canceled += StopJUp;
        joystickUp.action.performed += StartJUp;
        joystickDown.action.canceled += StopJDown;
        joystickDown.action.performed += StartJDown;
    }

    private void EyeUp(InputAction.CallbackContext context)
    {
        eyeVal += 5;
        eyeText.text = "20/" + eyeVal;
    }

    private void EyeDown(InputAction.CallbackContext context)
    {
        eyeVal -= 5;
        eyeText.text = "20/" + eyeVal;
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

    public void NextScene(InputAction.CallbackContext context)
    {
        // Iterate to the next scene if possible
        if (scene == sceneEnum.scene_end)
        {
            // End of scenes, close application
            Application.Quit();
            // For debug use in the Unity editor
            //EditorApplication.isPlaying = false;
            return;
        }

        // Get the scene name to see if it is a screen or line pair
        var sceneName = scene.ToString().Split("_");

        // Check if we're logging line pair data
        if (logData)
        {
            switch (sceneName[0])
            {
                case "lp":
                    Debug.Log(sceneName);
                    // Screenshot the current camera view
                    //ScreenCapture.CaptureScreenshot(dirPath + "/" + UUID + "-" + sceneIndex + ".png");
                    Debug.Log(dirPath + "/" + UUID + "-" + scene.ToString() + ".png");
                    // Write the current data to the text document
                    // Only log head data for dynamic tests
                    lp.LogData(true, scene > sceneEnum.scene_dynamic);
                    break;
                case "head":
                    // Move line pair to defined position to test head movement
                    
                    break;
            }
        }
        // Destroy the existing scene
        Destroy(sceneObj);
        // Iterate the scene index
        scene += 1;
        sceneName = scene.ToString().Split("_");
        // Set up the new scene
        if (sceneName[0] == "lp")
        {
            // Enable the joystick
            joystickUp.action.Enable();
            joystickDown.action.Enable();
            // Edit line pair
            drawNewLPs(sceneName);
        }
        else if (sceneName[0] == "head")
        {
            // Disable the joystick
            joystickUp.action.Disable();
            joystickDown.action.Disable();
            // Line tuning is off, update text
            instructionText.text = "Move your head perpendicular to the lines until they are no longer distinguishable";
        }
        else
        {
            // Draw the set menu
            drawNewMenu(sceneName);
        }
    }

    private void drawNewLPs(string[] sceneName)
    {
        // Set up instruction text
        instructionText.text = "Make the lines as small as possible while still being distinguishable";
        switch (sceneName.Last())
        {
            case "horizontal":
                // No rotation needed
                lp.MakeLines("HLP");
                break;
            case "vertical":
                lp.RotateTo(90);
                break;
            case "diagonal":
                lp.RotateTo(45);
                break;
        }
    }

    private void drawNewMenu(string[] sceneName)
    {
        // Delete the lines if possible and remove text
        instructionText.text = "";
        lp.Remove();
        // Show the right menu based on the name
        switch (sceneName.Last())
        {
            case "static":
            sceneObj = (GameObject)Instantiate(Resources.Load("Static Screen"));
                break;
            case "dynamic":
                // Enable head tracking
                staticCamera.SetActive(false);
                xrOrigin.SetActive(true);
                // Change parents
                sceneObj = (GameObject)Instantiate(Resources.Load("Dynamic Screen"));
                break;
            case "end":
                sceneObj = (GameObject)Instantiate(Resources.Load("End Screen"));
                break;
        }
    }

    void Update()
    {
        // Keep the current scene at the given position
        // lp.keepDistance();
        // Also update the line text
        instructionText.transform.position = new Vector3(0, -xrCamera.localPosition.z, 15);
        // Check for held values
        // UP
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
