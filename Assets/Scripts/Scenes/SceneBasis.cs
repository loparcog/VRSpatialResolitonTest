using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SceneBasis
{

    public Object sceneObject;
    public GameObject activeScene;
    public InputActionReference[] controllerButtons;

    public bool toDestroy = false;
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

    public abstract void RegisterControls();
    public abstract void DeregisterControls();

    // Update is called once per frame
    public virtual void Update()
    {
        return;
    }

    // Destroy scene and dereference controls
    public virtual void Destroy()
    {
        // By default, just call the deregister function
        DeregisterControls();
        Object.Destroy(activeScene);
    }
}
