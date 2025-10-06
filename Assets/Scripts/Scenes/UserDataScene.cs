using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserDataScene : SceneBasis
{
    private string UUID;
    private TextMeshPro eyeAcuityText;
    private TextMeshPro eyeDataText;
    // Stores visual acuity for left eye, right eye, and inter pupillary distance, respectively
    private int[] eyeVal = { 20, 20, 60, 0 };
    // LEFT = 0, RIGHT = 1, IPD = 2
    private int currEye = 0;
    public UserDataScene(InputActionReference[] controls, string userUUID) :
        base(Resources.Load("User Data Screen"), controls)
    {
        UUID = userUUID;
    }

    public override void Start()
    {
        base.Start();
        // Start logging
        LogController.Init(UUID);
        // Get UUID and active text
        var uuidText = GameObject.Find("UUID").GetComponent<TextMeshPro>().text = UUID;
        eyeAcuityText = GameObject.Find("Eye Acuity").GetComponent<TextMeshPro>();
        eyeDataText = GameObject.Find("Eye Data").GetComponent<TextMeshPro>();
        WriteEyeText();
    }

    public override void RegisterControls()
    {
        // Swap eyes
        controllerButtons[(int)Constants.CONTROLS.BUTTON].action.performed += ToggleDestroyFlag;
        controllerButtons[(int)Constants.CONTROLS.TRIGGER].action.performed += SwapEyeIndex;
        // Change prescription numbers with joysticks
        controllerButtons[(int)Constants.CONTROLS.UP].action.performed += EyeValueUp;
        controllerButtons[(int)Constants.CONTROLS.DOWN].action.performed += EyeValueDown;
    }
    private void WriteEyeText()
    {
        // Change formatting based on which field is edited
        switch (currEye)
        {
            case 0:
                // Left eye
                eyeAcuityText.text = "<color=yellow>Left Eye: 20/" + eyeVal[0] + "\n</color>" +
                    "Right Eye: 20/" + eyeVal[1] + "\n";
                eyeDataText.text = "IPD: " + eyeVal[2] + "mm\n" +
                    "Glasses: " + (eyeVal[3] == 0 ? "Yes" : "No");
                break;
            case 1:
                // Right eye
                eyeAcuityText.text = "Left Eye: 20/" + eyeVal[0] + "\n" +
                    "<color=yellow>Right Eye: 20/" + eyeVal[1] + "\n</color>";
                eyeDataText.text = "IPD: " + eyeVal[2] + "mm\n" +
                    "Glasses: " + (eyeVal[3] == 0 ? "Yes" : "No");
                break;
            case 2:
                // IPD
                eyeAcuityText.text = "Left Eye: 20/" + eyeVal[0] + "\n" +
                    "Right Eye: 20/" + eyeVal[1] + "\n";
                eyeDataText.text = "<color=yellow>IPD: " + eyeVal[2] + "mm</color>\n" +
                    "Glasses: " + (eyeVal[3] == 0 ? "Yes" : "No");
                break;
            case 3:
                // IPD
                eyeAcuityText.text = "Left Eye: 20/" + eyeVal[0] + "\n" +
                    "Right Eye: 20/" + eyeVal[1] + "\n";
                eyeDataText.text = "IPD: " + eyeVal[2] + "mm\n" +
                    "<color=yellow>Glasses: " + (eyeVal[3] == 0 ? "Yes" : "No") + "</color>";
                break;

        }
    }

    private void SwapEyeIndex(InputAction.CallbackContext context)
    {
        currEye++;
        currEye %= eyeVal.Length;
        WriteEyeText();
    }
    private void EyeValueUp(InputAction.CallbackContext context)
    {
        if (currEye == 2)
        {
            // IPD edit, iterate by 1
            eyeVal[currEye] += 1;
        }
        else if (currEye == 3)
        {
            // Glasses edit, 1 or 0
            eyeVal[currEye] = (eyeVal[currEye] + 1) % 2;
        }
        else
        {
            eyeVal[currEye] += 5;
            if (eyeVal[currEye] > 200)
            {
                eyeVal[currEye] = 200;
            }
        }
        // Rewrite the text to screen
        WriteEyeText();
    }

    private void EyeValueDown(InputAction.CallbackContext context)
    {
        if (currEye == 2)
        {
            // IPD edit
            eyeVal[currEye] -= 1;
        }
        else if (currEye == 3)
        {
            // Glasses edit, 1 or 0
            eyeVal[currEye] = (eyeVal[currEye] + 1) % 2;
        }
        else
        {
            eyeVal[currEye] -= 5;
            if (eyeVal[currEye] < 5)
            {
                eyeVal[currEye] = 5;
            }
        }
        // Rewrite the text to screen
        WriteEyeText();
    }

    public override void DeregisterControls()
    {
        // REMOVE EVERYTHING THAT WAS SET ABOVE
        controllerButtons[(int)Constants.CONTROLS.BUTTON].action.performed -= ToggleDestroyFlag;
        controllerButtons[(int)Constants.CONTROLS.TRIGGER].action.performed -= SwapEyeIndex;
        // Change prescription numbers with joysticks
        controllerButtons[(int)Constants.CONTROLS.UP].action.performed -= EyeValueUp;
        controllerButtons[(int)Constants.CONTROLS.DOWN].action.performed -= EyeValueDown;
    }

    public override void Destroy()
    {
        base.Destroy();
        // Write eye data to logs
        LogController.LogUserData(eyeVal[0], eyeVal[1], eyeVal[2], eyeVal[3]);
    }
}