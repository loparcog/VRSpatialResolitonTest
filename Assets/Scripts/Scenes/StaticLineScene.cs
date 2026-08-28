using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StaticLineScene : SceneBasis
{
    // Current test index
    // 0 = Horizontal, 1 = Vertical, 2 = Diagonal
    private Constants.LINE_ORIENTATION currTest = 0;
    // Line to instantiate
    // 0 = Infinite, 1 = E
    private Constants.LINE_TYPE currLine = 0;
    // Flag for E tests, primarily for logging
    // Line pair object
    private LinePair staticLinePair;
    // Instructions for line scaling
    private TextMeshPro instructionText;
    // Tools for line pair scaling
    private float[] UpDownTime = { 0, 0 };
    private bool[] UpDownHeld = { false, false };
    // Base object to add line pair system to
    private GameObject baseObject;
    private LogController log;

    public StaticLineScene(InputActionReference[] controls, LogController logger) :
        base(Resources.Load("Scenes/Static Screen"), controls)
    {
        // Save the log resource
        log = logger;
    }

    public override void Start()
    {
        base.Start();
        // ASSUMING LOGS ALREADY INITIATED IN USER INIT
        // Set up a line pair tool on an anchor object
        baseObject = new GameObject();
        staticLinePair = baseObject.AddComponent<LinePair>();
        // Create instruction text to be used later
        CreateInstructionText();

    }

    public override void Destroy()
    {
        base.Destroy();
        // Reset defaults
        currTest = 0;
        currLine = 0;
        // Remove all created game objects
        staticLinePair.Remove();
        Object.Destroy(baseObject);
        Object.Destroy(instructionText.gameObject);
    }

    private void CreateInstructionText()
    {
        var textObject = new GameObject();
        textObject.name = "Instruction Text";
        textObject.AddComponent<TextMeshPro>();
        textObject.transform.Rotate(90, 0, 0);
        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 5);
        instructionText = textObject.GetComponent<TextMeshPro>();
        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.fontSize = 20;
    }
    public override void RegisterControls()
    {
        base.RegisterControls();
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
        base.DeregisterControls();
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
        // If we're past the base case, log the data
        if (currTest > 0)
        {
            // Up the index by 3 if we're in the E tests
            log.LogLineData(staticLinePair.currentScale, currLine, currTest - 1);
        }

        // If we're passed the last orientation, swap to E test
        // Hardcoded for readability
        if (currTest > Constants.LINE_ORIENTATION.DIAGONAL & 
            currLine < Constants.LINE_TYPE.E)
        {
            // Iterate the current line test
            currLine++;
            // Reset values and remove existing lines
            currTest = 0;
            staticLinePair.Remove();
        }
        
        switch (currTest)
        {
            case Constants.LINE_ORIENTATION.HORIZONTAL:
                // No rotation needed
                staticLinePair.MakeLines("HLP " + currLine.ToString(), 0.5f);
                instructionText.transform.position = new Vector3(0, 0, 10);
                break;
            case Constants.LINE_ORIENTATION.VERTICAL:
                staticLinePair.RotateTo(90);
                instructionText.transform.position = new Vector3(-15, 0, 0);
                break;
            case Constants.LINE_ORIENTATION.DIAGONAL:
                staticLinePair.RotateTo(135);
                instructionText.transform.position = new Vector3(-10, 0, 10);
                break;
            default:
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
        staticLinePair.IncreaseSize(controllerButtons[(int)Constants.CONTROLS.TRIGGER].action.inProgress);
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
        staticLinePair.DecreaseSize(controllerButtons[(int)Constants.CONTROLS.TRIGGER].action.inProgress);
        // Start adding to time
        UpDownHeld[1] = true;
    }
    
    public override void Update()
    {
        // Check for held values
        // UP
        if (UpDownHeld[0])
        {
            // Add delta time (done in seconds)
            UpDownTime[0] += Time.deltaTime;
            if (UpDownTime[0] > 0.5)
            {
                // Repeatedly increase size
                staticLinePair.IncreaseSize(true);
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
                staticLinePair.DecreaseSize(true);
            }
        }
    }
}