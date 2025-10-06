
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneController : MonoBehaviour
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
    private SceneBasis[] sceneList;
    private InputActionReference[] controllerButtons;
    private int sceneIndex = 0;
    // Tools for current scene management
    private GameObject currentScene;
    // Data for screenshotting and file writing
    private string UUID = System.Guid.NewGuid().ToString();

    void Start()
    {
        // Declare the controls list, abiding by the enum in Constants.cs
        controllerButtons = new InputActionReference[] {
            joystickUp,
            joystickDown,
            primaryButton,
            triggerButton
        };
        // Initialize the scene list
        sceneList = new SceneBasis[] {
            new StartScene(controllerButtons),
            new TutorialScene(controllerButtons),
            new UserDataScene(controllerButtons, UUID),
            new StaticLineScene(controllerButtons),
            new DynamicLineScene(controllerButtons, staticCamera, xrCamera, xrOrigin),
        };
        print(sceneList);
        // Build the first scene
        ConstructScene();

    }

    // Build the scene for the current sceneIndex
    void ConstructScene()
    {
        // Make sure the scene exists
        if (sceneIndex > sceneList.Length)
        {
            // End the program here
            Application.Quit();
            // For debug use in the Unity editor
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
        sceneList[sceneIndex].Start();
    }

    void Update()
    {
        SceneBasis cS = sceneList[sceneIndex];
        // Check the current scene update function
        cS.Update();
        // See if the deletion flag is open
        if (cS.toDestroy)
        {
            // Run the destroy function
            cS.Destroy();
            // Iterate to the next scene
            sceneIndex++;
            // Construct the new scene
            ConstructScene();
        }
    }
}
