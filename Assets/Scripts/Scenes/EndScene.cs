using UnityEngine;
using UnityEngine.InputSystem;

public class EndScene : SceneBasis
{
    private LogController log;
    public EndScene(InputActionReference[] controls, LogController logger) :
        base(Resources.Load("End Screen"), controls)
    {
        log = logger;
    }

    public override void Start()
    {
        base.Start();
        // Write to the file
        log.WriteToFile();
    }

    public override void RegisterControls()
    {
        base.RegisterControls();
        // Set the destruction flag
        controllerButtons[(int)Constants.CONTROLS.BUTTON].action.performed += ToggleDestroyFlag;
    }
    public override void DeregisterControls()
    {
        base.DeregisterControls();
        // REMOVE EVERYTHING THAT WAS SET ABOVE
        controllerButtons[(int)Constants.CONTROLS.BUTTON].action.performed -= ToggleDestroyFlag;
    }
}