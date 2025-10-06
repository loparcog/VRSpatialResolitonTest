using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicLineScene : SceneBasis
{
    // Current test index
    // 0 = Horizontal, 1 = Vertical, 2 = Diagonal
    private int currTest = 0;
    // Line pair object
    private LinePair dynamicLinePair;
    // Instructions for line scaling
    private TextMeshPro instructionText;
    // Tools for line pair scaling
    private float[] UpDownTime = { 0, 0 };
    private bool[] UpDownHeld = { false, false };
    // Base object to add line pair system to
    private GameObject baseObject;
    // Camera references
    private GameObject staticCamera;
    private Transform xrCamera;
    private GameObject xrOrigin;



    public DynamicLineScene(InputActionReference[] controls, GameObject staticCam, Transform dynamicCam, GameObject xrO) :
        base(Resources.Load("Dynamic Screen"), controls)
    {
        // Save camera references for later use
        staticCamera = staticCam;
        xrCamera = dynamicCam;
        xrOrigin = xrO;
    }
    
    public override void Start()
    {
        base.Start();
        // ASSUMING LOGS ALREADY INITIATED IN USER INIT
        // Swap cameras to use
        staticCamera.SetActive(false);
        xrOrigin.SetActive(true);
        // Set up a line pair tool on an anchor object
        baseObject = new GameObject();
        dynamicLinePair = baseObject.AddComponent<LinePair>();
        dynamicLinePair.SetCamera(xrCamera);
        // Create instruction text to be used later
        CreateInstructionText();

    }

    public override void Destroy()
    {
        base.Destroy();
        // Remove all created game objects
        dynamicLinePair.Remove();
        Object.Destroy(baseObject);
        Object.Destroy(instructionText.gameObject);
    }

    private void CreateInstructionText()
    {
        var textObject = new GameObject();
        textObject.name = "Instruction Text";
        textObject.AddComponent<TextMeshPro>();
        textObject.transform.Rotate(90, 0, 0);
        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 5);
        instructionText = textObject.GetComponent<TextMeshPro>();
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.fontSize = 20;
    }
    public override void RegisterControls()
    {
        // Next scene
        controllerButtons[(int)Constants.CONTROLS.BUTTON].action.performed += NextTest;
        // Joystick line scaling
        controllerButtons[(int)Constants.CONTROLS.UP].action.canceled += StopJUp;
        controllerButtons[(int)Constants.CONTROLS.UP].action.performed += StartJUp;
        controllerButtons[(int)Constants.CONTROLS.DOWN].action.canceled += StopJDown;
        controllerButtons[(int)Constants.CONTROLS.DOWN].action.performed += StartJDown;
    }
    public override void DeregisterControls()
    {
        // REMOVE EVERYTHING THAT WAS SET ABOVE
        controllerButtons[(int)Constants.CONTROLS.BUTTON].action.performed -= NextTest;
        controllerButtons[(int)Constants.CONTROLS.UP].action.canceled -= StopJUp;
        controllerButtons[(int)Constants.CONTROLS.UP].action.performed -= StartJUp;
        controllerButtons[(int)Constants.CONTROLS.DOWN].action.canceled -= StopJDown;
        controllerButtons[(int)Constants.CONTROLS.DOWN].action.performed -= StartJDown;
    }

    private void NextTest(InputAction.CallbackContext context)
    {
        // Destroy the existing scene
        Object.Destroy(activeScene);
        // Iterate through each test based on the test ID
        instructionText.text = "Make the lines as small as possible while still being distinguishable";
        if (currTest > 0)
        {
            LogController.LogLineData(dynamicLinePair.currentScale);
        }
        switch (currTest)
        {
            case 0:
                // No rotation needed
                dynamicLinePair.MakeLines("HLP Infinite");
                instructionText.transform.position = new Vector3(0, 0, 15);
                break;
            case 1:
                dynamicLinePair.RotateTo(90);
                instructionText.transform.position = new Vector3(-20, 0, 0);
                break;
            case 2:
                dynamicLinePair.RotateTo(45);
                instructionText.transform.position = new Vector3(-20, 0, 15);
                break;
            default:
                // Scene finished, toggle flag
                ToggleDestroyFlag();
                break;
        }
        // Iterate the current test
        currTest++;
    }

    private void StopJUp(InputAction.CallbackContext context)
    {
        UpDownHeld[0] = false;
        UpDownTime[0] = 0;
    }

    private void StartJUp(InputAction.CallbackContext context)
    {
        // Perform base action
        dynamicLinePair.IncreaseSize(controllerButtons[(int)Constants.CONTROLS.TRIGGER].action.inProgress);
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
        dynamicLinePair.DecreaseSize(controllerButtons[(int)Constants.CONTROLS.TRIGGER].action.inProgress);
        // Start adding to time
        UpDownHeld[1] = true;
    }
    
    public override void Update()
    {
        // Keep the current scene at the given position
        dynamicLinePair.keepDistance();
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
                dynamicLinePair.IncreaseSize(true);
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
                dynamicLinePair.DecreaseSize(true);
            }
        }
    }
}