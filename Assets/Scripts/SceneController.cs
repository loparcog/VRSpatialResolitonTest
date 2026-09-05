using UnityEngine;
using UnityEngine.InputSystem;

/*
    SCENE CONTROLLER
    Overarching director for all tests and scenes. Can be found in
    the editor on the Script Manager object
*/
public class SceneController : MonoBehaviour
{
    // Camera objects
    [SerializeField] public GameObject staticCamera;
    [SerializeField] public GameObject xrOrigin;
    [SerializeField] public Transform xrCamera;
    // Controller input actions
    [SerializeField] public InputActionReference primaryButton;
    [SerializeField] public InputActionReference secondaryButton;
    [SerializeField] public InputActionReference triggerButton;
    [SerializeField] public InputActionReference joystickUp;
    [SerializeField] public InputActionReference joystickDown;
    [SerializeField] public bool printDebug;
    // Log manager
    private LogController log = new LogController();
    private SceneBasis[] sceneList;
    private InputActionReference[] controllerButtons;
    private int sceneIndex = 0;
    // Tools for current scene management
    private GameObject currentScene;
    // Data for screenshotting and file writing
    private string UUID = System.Guid.NewGuid().ToString();

    void Start()
    {
        // Declare the controls list, abiding by the enum order in Constants.cs
        controllerButtons = new InputActionReference[] {
            joystickUp,
            joystickDown,
            primaryButton,
            secondaryButton,
            triggerButton
        };
        // Initialize the scene list
        sceneList = new SceneBasis[] {
            new StartScene(controllerButtons),
            new TutorialScene(controllerButtons),
            new UserDataScene(controllerButtons, log, UUID),
            new StaticLineScene(controllerButtons, log),
            new DynamicLineScene(controllerButtons, log, staticCamera, xrCamera, xrOrigin),
            new EndScene(controllerButtons, log),
        };
        // Initialize the log with the user UUID and debug option
        log.Init(UUID, printDebug);
        // Build the first scene
        ConstructScene();

    }

    // Build the scene for the current sceneIndex
    void ConstructScene()
    {
        // Make sure the scene exists
        if (sceneIndex >= sceneList.Length)
        {
            // End the program
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
            return;
        }
        // Call the custom start script for the selected scene
        sceneList[sceneIndex].Start();
    }

    void Update()
    {
        SceneBasis currentScene = sceneList[sceneIndex];
        // Check the current scene update function
        currentScene.Update();
        // See if the deletion flag is open
        if (currentScene.toDestroy)
        {
            // See which direction to go for the scene
            if (currentScene.goBack)
            {
                sceneIndex--;
            }
            else
            {
                sceneIndex++;
            }
            // Run the destroy function
            currentScene.Destroy();
            // Construct the new scene
            ConstructScene();
        }
    }
}
