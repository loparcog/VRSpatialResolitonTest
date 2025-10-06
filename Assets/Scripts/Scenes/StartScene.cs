using UnityEngine;
using UnityEngine.InputSystem;

public class StartScene : SceneBasis
{
    public StartScene(InputActionReference[] controls) :
        base(Resources.Load("Start Screen"), controls) { }
    public override void RegisterControls()
    {
        // Set the destruction flag
        controllerButtons[(int)Constants.CONTROLS.BUTTON].action.performed += ToggleDestroyFlag;
    }
    public override void DeregisterControls()
    {
        // REMOVE EVERYTHING THAT WAS SET ABOVE
        controllerButtons[(int)Constants.CONTROLS.BUTTON].action.performed -= ToggleDestroyFlag;
    }
}