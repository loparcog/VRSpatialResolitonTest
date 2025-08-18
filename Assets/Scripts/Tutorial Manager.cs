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
    private string[] scenes = { "start", "tutorial" };
    private int sceneIndex = 0;
    private GameObject currentScene;
    private LinePair lp;

    void Start()
    {
        // Create the line pair for future use
        lp = this.AddComponent<LinePair>();
        // Only bind the nextScene button
        primaryButton.action.performed += NextScene;
        // Show the initial info screen
        currentScene = (GameObject)Instantiate(Resources.Load("Start Screen"));
    }

    public void NextScene(InputAction.CallbackContext context)
    {

        // Iterate to the next scene if possible
        if (sceneIndex == scenes.Length - 1)
        {
            // End of scenes, remove controls and continue to the line tester
            DeregisterControls();
            SceneManager.LoadScene(1);
            return;
        }

        // Destroy the existing scene
        Destroy(currentScene);
        // Iterate the scene index
        sceneIndex += 1;
        // Set up the new scene
        buildTutorial();
    }
    private void buildTutorial()
    {
        // Set up scene and line pair
        currentScene = (GameObject)Instantiate(Resources.Load("Tutorial Screen"));
        lp.MakeLines(0.3f);
        lp.lines.transform.position = new Vector3(10, 0, 0);
        // Register controls to change the line movement
        RegisterControls();
    }

    void RegisterControls()
    {
        // Register line changing controls
        joystickUp.action.performed += IncreaseLPSize;
        joystickDown.action.performed += DecreaseLPSize;
        triggerButton.action.started += lp.FineTuneEnabled;
        triggerButton.action.canceled += lp.FineTuneDisabled;
    }

    void DeregisterControls()
    {
        // Remove NextScene button as well
        primaryButton.action.performed -= NextScene;
        joystickUp.action.performed -= IncreaseLPSize;
        joystickDown.action.performed -= DecreaseLPSize;
        triggerButton.action.started -= lp.FineTuneEnabled;
        triggerButton.action.canceled -= lp.FineTuneDisabled;
    }

    void IncreaseLPSize(InputAction.CallbackContext context)
    {
        // No check needed here, only used when line pairs are visible
        lp.IncreaseSize();
    }

    void DecreaseLPSize(InputAction.CallbackContext context)
    {
        lp.DecreaseSize();
    }
}
