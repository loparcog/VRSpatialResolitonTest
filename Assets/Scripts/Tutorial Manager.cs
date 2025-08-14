using System.IO;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    // Scene prefabs
    [SerializeField] public GameObject startScreen;
    [SerializeField] public GameObject tutorialScreen;
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
    private int sceneIndex = 0;
    private GameObject currentScene;
    // Information on the current line scaling
    private float currentScale = 1.0f;
    private bool fineScale = false;

    void Start()
    {
        // Only bind the nextScene button
        primaryButton.action.performed += NextScene;
        // Show the initial info screen
        currentScene = Instantiate(startScreen);
    }

    public void NextScene(InputAction.CallbackContext context)
    {

        // Iterate to the next scene if possible
        if (sceneIndex == scenes.Length - 1)
        {
            // End of scenes, continue to the line tester
            return;
        }

        // Destroy the existing scene
        Destroy(currentScene);
        // Iterate the scene index
        sceneIndex += 1;
        // Set up the new scene
        drawNewMenu();
    }
    private void drawNewMenu()
    {
        // TODO
        return;
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
}
