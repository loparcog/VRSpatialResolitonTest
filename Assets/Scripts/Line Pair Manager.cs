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
    [SerializeField] public GameObject XRCamera;
    // Scene prefabs
    [SerializeField] public GameObject staticScreen;
    [SerializeField] public GameObject dynamicScreen;
    [SerializeField] public GameObject endScreen;
    [SerializeField] public GameObject horizontalLP;
    // Controller input actions
    [SerializeField] public InputActionReference primaryButton;
    [SerializeField] public InputActionReference triggerButton;
    [SerializeField] public InputActionReference joystickUp;
    [SerializeField] public InputActionReference joystickDown;
    [SerializeField] public bool logData = false;
    // Scenes, in order
    private string[] scenes = { "scene_static", "lp_horizontal", "lp_vertical", "lp_diagonal", "scene_dynamic", "lp_horizontal", "lp_vertical", "lp_diagonal", "scene_end" };
    private int sceneIndex = 0;
    // Information on the current line scaling
    private float currentScale = 0.5f;
    private bool fineScale = false;
    // Tools for current scene management
    private GameObject currentScene;
    private Transform currentCamera;
    // Data for screenshotting and file writing
    private string UUID = System.Guid.NewGuid().ToString();
    private string filePath = "VRRTData.csv";
    private string dirPath = "VRRT Data";

    void Start()
    {
        // Set the scene control bindings
        primaryButton.action.performed += NextScene;
        joystickUp.action.performed += IncreaseLPSize;
        joystickDown.action.performed += DecreaseLPSize;
        triggerButton.action.started += FineTuneEnabled;
        triggerButton.action.canceled += FineTuneDisabled;
        // Set up data logging if toggled
        if (logData)
        {
            // Update the file and directory paths to accomodate the current application path
            // Can't use Path.Combine() since it gets funky with the slashes
            dirPath = Application.persistentDataPath + "/" + dirPath;
            filePath = dirPath + "/" + filePath;

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
        // Set the current parent
        currentCamera = staticCamera.transform;
        // Show the start screen
        currentScene = Instantiate(staticScreen);
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
        if (logData & sceneName[0] == "lp")
        {
            Debug.Log(sceneName);
            // Screenshot the current camera view
            ScreenCapture.CaptureScreenshot(dirPath + "/" + UUID + "-" + sceneIndex + ".png");
            Debug.Log(dirPath + "/" + UUID + "-" + sceneIndex + ".png");
            // Write the current data to the text document
            using (StreamWriter sw = File.AppendText(filePath))
            {
                // Write the line size
                sw.Write("," + currentScale.ToString("F3") + "mm");
                if (sceneIndex > 4)
                {
                    // Also save head rotation position (horizontal + vertical)
                    sw.Write("," + currentCamera.localEulerAngles.y.ToString("F3") + "/" + currentCamera.localEulerAngles.x.ToString("F3"));
                }
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
        // Create the static lines
        // (Five line pairs with differenting sizes)
        currentScene = Instantiate(horizontalLP);
        currentScene.name = "Line Pairs";
        // Scale the scene to match the last point
        currentScene.transform.localScale = new Vector3(1, 1, currentScale);
        // Change the scene based on the setup
        switch (sceneName[1])
        {
            case "horizontal":
                // No rotation needed
                break;
            case "vertical":
                currentScene.transform.Rotate(0, 90, 0);
                break;
            case "diagonal":
                currentScene.transform.Rotate(0, 45, 0);
                break;
        }
    }

    private void drawNewMenu(string[] sceneName)
    {
        // Show the right menu based on the name
        switch (sceneName[1])
        {
            case "static":
                // Show the static menu
                currentScene = Instantiate(staticScreen);
                break;
            case "dynamic":
                // Enable head tracking
                staticCamera.SetActive(false);
                XRCamera.SetActive(true);
                // Change parents
                currentCamera = XRCamera.transform.GetChild(0).GetChild(0);
                currentScene = Instantiate(dynamicScreen);
                break;
            case "end":
                currentScene = Instantiate(endScreen);
                break;
        }
    }


    void IncreaseLPSize(InputAction.CallbackContext context)
    {
        // Make sure its a line pair scene
        if (scenes[sceneIndex].Split("_")[0] == "scene") return;
        // Check if fine zoom is enabled (change by 0.001mm)
        if (fineScale)
        {
            currentScale += 0.001f;
        }
        // Otherwise just scale by 0.01m
        else
        {
            currentScale += 0.01f;
        }
        // Limit scale up
        if (currentScale > 0.5f) currentScale = 0.5f;
        // Apply the current scale
        currentScene.transform.localScale = new Vector3(1, 1, currentScale);
    }

    void DecreaseLPSize(InputAction.CallbackContext context)
    {
        if (scenes[sceneIndex].Split("_")[0] == "scene") return;
        if (fineScale)
        {
            currentScale -= 0.001f;
        }
        else
        {
            currentScale -= 0.01f;
        }
        // Limit zoom in
        if (currentScale < 0f) currentScale = 0f;
        currentScene.transform.localScale = new Vector3(1, 1, currentScale);

    }

    void FineTuneEnabled(InputAction.CallbackContext context)
    {
        fineScale = true;
    }
    void FineTuneDisabled(InputAction.CallbackContext context)
    {
        fineScale = false;
    }

    void Update()
    {
        // Keep the current scene at the given position
        currentScene.transform.position = new Vector3(0, -currentCamera.localPosition.z, 0);
    }
}
