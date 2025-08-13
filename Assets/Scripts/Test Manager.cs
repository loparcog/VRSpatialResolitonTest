using System;
using System.IO;
using NUnit.Framework.Constraints;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class TestManager : MonoBehaviour
{
    // Camera objects
    [SerializeField] public GameObject staticCamera;
    [SerializeField] public GameObject XRCamera;
    // Scene prefabs
    [SerializeField] public GameObject startScreen;
    [SerializeField] public GameObject controlScreen;
    [SerializeField] public GameObject staticScreen;
    [SerializeField] public GameObject dynamicScreen;
    [SerializeField] public GameObject endScreen;
    [SerializeField] public GameObject horizontalLP;
    [SerializeField] public Material highlightMaterial;
    // Controller input actions
    [SerializeField] public InputActionReference primaryButton;
    [SerializeField] public InputActionReference secondaryButton;
    [SerializeField] public InputActionReference triggerButton;
    [SerializeField] public InputActionReference joystickUp;
    [SerializeField] public InputActionReference joystickDown;
    [SerializeField] public bool logData = false;
    // Scenes, in order
    private string[] scenes = { "scene_start", "scene_control", "scene_static", "lp_horizontal", "lp_vertical", "lp_diagonal", "scene_dynamic",  "lp_horizontal", "lp_vertical", "lp_diagonal", "scene_end"};
    private int sceneIndex = 0;
    // Information on the current line scaling
    private float currentScale = 0.5f;
    private bool fineZoom = false;
    private GameObject currentScene;
    // Data for screenshotting and file writing
    private string UUID = System.Guid.NewGuid().ToString();
    private string filePath = "VRRTData.csv";
    private string dirPath = "VRRT Data";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Create bindings for the primary buttons
        primaryButton.action.performed += NextScene;
        joystickUp.action.performed += IncreaseLPSize;
        joystickDown.action.performed += DecreaseLPSize;        
        triggerButton.action.started += FineTuneEnabled;
        triggerButton.action.canceled += FineTuneDisabled;
        // Initialize log information if needed
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
                    // S_ = Static, D_ = Dynamic (head tracking)
                    // _H, _V, _D = Horizontal, Vertical, Diagonal
                    sw.WriteLine("UUID,SH,SV,SD,DH,DV,DD");
                    sw.Write(UUID);
                }
            }
            else
            {
                // Just write the UUID
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.Write(UUID);
                }
            }
            Debug.Log("Data being saved to: " + filePath);
        }
        // Show the start screen
        currentScene = Instantiate(startScreen);
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

        var sceneName = scenes[sceneIndex].Split("_");

        // Take a screenshot if in a line pair scene
        if (logData & sceneName[0] == "lp")
        {
            // Screenshot the current camera view
            ScreenCapture.CaptureScreenshot(dirPath + "/" + UUID + "-" + sceneIndex + ".png");
            Debug.Log(dirPath + "/" + UUID + "-" + sceneIndex + ".png");
            // Write the current data to the text document
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.Write("," + currentScale.ToString("F3") + "mm");
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
        currentScene = new GameObject();
        currentScene.name = "Line Pairs";
        for (int i = -1; i < 2; i++)
        {
            var linePair = Instantiate(horizontalLP);
            var scale = (float)(1 + (i * 0.01));
            linePair.transform.localScale = new Vector3(1, 1, scale);
            linePair.transform.Translate(0, 0, i * (scale * 20));
            // Highlight the middle pair
            if (i == 0)
            {
                foreach (Transform child in linePair.transform)
                {
                    child.GetComponent<Renderer>().sharedMaterial = highlightMaterial;
                }
            }
            linePair.transform.parent = currentScene.transform;
        }
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
            case "control":
                // Show the control menu and set up bindings
                currentScene = Instantiate(controlScreen);
                break;
            case "static":
                // Show the static menu
                currentScene = Instantiate(staticScreen);
                break;
            case "dynamic":
                // Show the menu and enable head tracking
                currentScene = Instantiate(dynamicScreen);
                staticCamera.SetActive(false);
                XRCamera.SetActive(true);
                // Also log this change
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
        // Check for a fine zoom
        if (fineZoom)
        {
            currentScale += 0.001f;
        }
        else
        {
            currentScale += 0.01f;
        }
        // Limit zoom out
        if (currentScale > 1f) currentScale = 1f;
        currentScene.transform.localScale = new Vector3(1, 1, currentScale);
    }

    void DecreaseLPSize(InputAction.CallbackContext context)
    {
        if (scenes[sceneIndex].Split("_")[0] == "scene") return;
        if (fineZoom)
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
        fineZoom = true;
    }
    void FineTuneDisabled(InputAction.CallbackContext context)
    {
        fineZoom = false;
    }

}
