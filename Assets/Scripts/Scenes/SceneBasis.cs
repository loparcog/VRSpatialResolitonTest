using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SceneBasis
{

    public Object sceneObject;
    public GameObject activeScene;
    public InputActionReference[] controllerButtons;

    public bool toDestroy = false;
    public bool goBack = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SceneBasis(Object scene, InputActionReference[] controls)
    {
        // Store data locally for future referencing
        sceneObject = scene;
        controllerButtons = controls;
    }

    // For inputs from controller
    public void ToggleDestroyFlag(InputAction.CallbackContext context)
    {
        toDestroy = true;
    }

    public void ToggleBackwardsDestroyFlag(InputAction.CallbackContext context)
    {
        // Destroy and set backwards flag
        toDestroy = true;
        goBack = true;
    }

    // For manual use
    public void ToggleDestroyFlag()
    {
        toDestroy = true;
    }

    public virtual void Start()
    {
        // Register the controls
        RegisterControls();
        // Show the given scene
        activeScene = (GameObject)Object.Instantiate(sceneObject);
    }

    public virtual void RegisterControls()
    {
        controllerButtons[(int)Constants.CONTROLS.SECONDBUTTON].action.performed += ToggleBackwardsDestroyFlag;
    }
    public virtual void DeregisterControls()
    {
        controllerButtons[(int)Constants.CONTROLS.SECONDBUTTON].action.performed -= ToggleBackwardsDestroyFlag;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        return;
    }

    // Destroy scene and dereference controls
    public virtual void Destroy()
    {
        // Turn off the go back and destroy flags
        goBack = false;
        toDestroy = false;
        // By default, just call the deregister function
        DeregisterControls();
        Object.Destroy(activeScene);
    }
}
