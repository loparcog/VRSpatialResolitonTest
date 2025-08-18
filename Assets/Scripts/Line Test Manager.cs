using System.IO;
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
    private string[] scenes = { "scene_static", "lp_horizontal", "lp_vertical", "lp_diagonal", "scene_dynamic", "lp_horizontal", "head_horizontal", "lp_vertical", "head_vertical", "lp_diagonal", "scene_end" };
    private int sceneIndex = 0;
    // Tools for current scene management
    private GameObject currentScene;
    private LinePair lp;
    private TextMeshPro instructionText;
    // Data for screenshotting and file writing
    private string UUID = System.Guid.NewGuid().ToString();
    // Update the file and directory paths to accomodate the current application path
    // Can't use Path.Combine() since it gets funky with the slashes
    private string dirPath = "VRRT Data";
    private string filePath = "VRRTData.csv";

    void Start()
    {
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
        // Set up action bindings
        primaryButton.action.performed += NextScene;
        joystickUp.action.performed += IncreaseLPSize;
        joystickDown.action.performed += DecreaseLPSize;
        triggerButton.action.started += lp.FineTuneEnabled;
        triggerButton.action.canceled += lp.FineTuneDisabled;
        // Show the start screen
        currentScene = (GameObject)Instantiate(Resources.Load("Static Screen"));
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

    public void NextScene(InputAction.CallbackContext context)
    {

        // Iterate to the next scene if possible
        if (sceneIndex == scenes.Length - 1)
        {
            // End of scenes, close application
            Application.Quit();
            // For debug use in the Unity editor
            EditorApplication.isPlaying = false;
            return;
        }

        // Get the scene name to see if it is a screen or line pair
        var sceneName = scenes[sceneIndex].Split("_");

        // Check if we're logging line pair data
        if (logData)
        {
            switch (sceneName[0])
            {
                case "lp":
                    Debug.Log(sceneName);
                    // Screenshot the current camera view
                    ScreenCapture.CaptureScreenshot(dirPath + "/" + UUID + "-" + sceneIndex + ".png");
                    Debug.Log(dirPath + "/" + UUID + "-" + sceneIndex + ".png");
                    // Write the current data to the text document
                    // Only log head data for dynamic tests
                    lp.LogData(true, sceneIndex > 4);
                    break;
                case "head":
                    // Just log head positioning
                    lp.LogData(false, true);
                    break;
            }
        }
        // Destroy the existing scene
        Destroy(currentScene);
        // Iterate the scene index
        sceneIndex += 1;
        sceneName = scenes[sceneIndex].Split("_");
        // Set up the new scene
        if (sceneName[0] == "lp")
        {
            // Edit line pair
            drawNewLPs(sceneName);
        }
        else if (sceneName[0] == "head")
        {
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
        switch (sceneName[1])
        {
            case "horizontal":
                // No rotation needed
                lp.MakeLines();
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
        switch (sceneName[1])
        {
            case "dynamic":
                // Enable head tracking
                staticCamera.SetActive(false);
                xrOrigin.SetActive(true);
                // Change parents
                currentScene = (GameObject)Instantiate(Resources.Load("Dynamic Screen"));
                break;
            case "end":
                currentScene = (GameObject)Instantiate(Resources.Load("End Screen"));
                break;
        }
    }

    private void IncreaseLPSize(InputAction.CallbackContext context)
    {
        if (scenes[sceneIndex].Split("_")[0] == "lp") lp.IncreaseSize();
    }

    private void DecreaseLPSize(InputAction.CallbackContext context)
    {
        if (scenes[sceneIndex].Split("_")[0] == "lp") lp.DecreaseSize();
        
    }
    void Update()
    {
        // Keep the current scene at the given position
        lp.keepDistance();
        // Also update the line text
        instructionText.transform.position = new Vector3(0, -xrCamera.localPosition.z, 15);
    }
}
