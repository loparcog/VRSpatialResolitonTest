using System.IO;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class LinePairManager : MonoBehaviour
{
    // Camera objects
    [SerializeField] public GameObject staticCamera;
    [SerializeField] public GameObject xrOrigin;
    [SerializeField] public GameObject xrCamera;
    // Controller input actions
    [SerializeField] public InputActionReference primaryButton;
    [SerializeField] public InputActionReference triggerButton;
    [SerializeField] public InputActionReference joystickUp;
    [SerializeField] public InputActionReference joystickDown;
    [SerializeField] public bool logData = false;
    // Scenes, in order
    private string[] scenes = { "scene_static", "lp_horizontal", "lp_vertical", "lp_diagonal", "scene_dynamic", "lp_horizontal", "head_horizontal", "lp_vertical", "head_vertical", "lp_diagonal", "head_diagonal", "scene_end" };
    private int sceneIndex = 0;
    // Tools for current scene management
    private GameObject currentScene;
    private GameObject lp;
    // Data for screenshotting and file writing
    private string UUID = System.Guid.NewGuid().ToString();
    // Update the file and directory paths to accomodate the current application path
    // Can't use Path.Combine() since it gets funky with the slashes
    private string dirPath = Application.persistentDataPath + "/" + "VRRT Data";
    private string filePath = dirPath + "/" + "VRRTData.csv";

    void Start()
    {
        // Set up data logging if toggled
        if (logData)
        {

            // Make sure the screenshot folder and text document exists
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            if (!File.Exists(filePath))
            {
                // Set up the CSV
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    // UUID for the user
                    // S_ = Static, D_ = Dynamic (head tracking), HP = Head Position
                    // _H, _V, _D = Horizontal, Vertical, Diagonal
                    sw.WriteLine("UUID,SH,SV,SD,DH,HPH,DV,HPV,DD,HPD");
                    sw.Write(UUID);
                }
            }
            else
            {
                // Just write the UUID
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.Write("\n" + UUID);
                }
            }
            Debug.Log("Data being saved to: " + filePath);
        }
        // Set up a line pair tool
        lp = new LinePair(xrCamera, filePath);
        primaryButton.action.performed += NextScene;
        joystickUp.action.performed += lp.IncreaseSize;
        joystickDown.action.performed += lp.DecreaseSize;
        triggerButton.action.started += lp.FineTuneEnabled;
        triggerButton.action.canceled += lp.FineTuneDisabled;
        // Show the start screen
        currentScene = Instantiate(Resources.Load("Static Screen"));
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
                    lp.logData(true, sceneIndex > 4);
                    break;
                case "head":
                    // Just log head positioning
                    lp.logData(false, true);
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
            drawNewLPs(sceneName);
        }
        else
        {
            drawNewMenu(sceneName);
        }
    }

    private void drawNewLPs(string[] sceneName)
    {
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
        // Show the right menu based on the name
        switch (sceneName[1])
        {
            case "dynamic":
                // Enable head tracking
                staticCamera.SetActive(false);
                xrOrigin.SetActive(true);
                // Change parents
                currentScene = Instantiate(Resources.Load("Dynamic Screen"));
                break;
            case "end":
                currentScene = Instantiate(Resources.Load("End Screen"));
                break;
        }
    }
    void Update()
    {
        // Keep the current scene at the given position
        currentScene.transform.position = new Vector3(0, -xrCamera.localPosition.z, 0);
    }
}
