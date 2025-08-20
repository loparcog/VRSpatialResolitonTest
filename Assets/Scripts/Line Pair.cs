using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class LinePair : MonoBehaviour
{
    // Horizontal line pair (1mm lines)
    public GameObject lines;
    private Transform xrCamera;
    // Current scale
    private float currentScale = 0.5f;
    // File for logging
    private string logFile;

    const float LINE_MAX = 0.8f;

    public void SetCamera(Transform camera)
    {
        // Save camera for head rotation
        xrCamera = camera;
    }

    public void MakeLines(float scale = LINE_MAX)
    {
        // Instantiate the line pair
        lines = Instantiate(Resources.Load<GameObject>("HLP"));
        lines.name = "Line Pairs";
        // Reset the current scale
        currentScale = scale;
        // Scale the scene to match the scale point
        lines.transform.localScale = new Vector3(1, 1, currentScale);
    }

    public void RotateTo(float angle)
    {
        lines.transform.Rotate(0, angle, 0);
    }

    public void IncreaseSize(bool fineScale)
    {
        // Make sure lines exist
        if (lines == null) return;
        // Check if fine zoom is enabled (change by 0.001mm)
        if (fineScale)
        {
            currentScale += 0.001f;
        }
        // Otherwise just scale by 0.01cm
        else
        {
            currentScale += 0.01f;
        }
        // Limit scale up
        if (currentScale > LINE_MAX) currentScale = LINE_MAX;
        // Apply the current scale
        lines.transform.localScale = new Vector3(1, 1, currentScale);
    }

    public void DecreaseSize(bool fineScale)
    {
        if (lines == null) return;
        if (fineScale)
        {
            currentScale -= 0.001f;
        }
        else
        {
            currentScale -= 0.01f;
        }
        // Limit scale down
        if (currentScale < 0f) currentScale = 0f;
        lines.transform.localScale = new Vector3(1, 1, currentScale);

    }

    public void keepDistance()
    {
        if (lines)
        {
            lines.transform.position = new Vector3(0, -xrCamera.localPosition.z, 0);
        }
    }

    public void InitLog(string dirPath, string filePath, string UUID)
    {
        // Store the file path for future logging
        logFile = filePath;
        // Make sure the screenshot folder and text document exists
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
        if (!File.Exists(logFile))
        {
            // Set up the CSV
            using (StreamWriter sw = new StreamWriter(logFile))
            {
                // UUID for the user
                sw.WriteLine("UUID,Static Horizontal,Static Vertical,Static Diagonal,Dynamic Hoizontal,Headpos Horizontal,Negative Headpos Horizontal,Dynamic Vertical,HeadPos Vertical,Negative Headpos Vertical,Dynamic Diagonal,Headpos Diagonal");
                sw.Write(UUID);
            }
        }
        else
        {
            // Just write the UUID
            using (StreamWriter sw = File.AppendText(logFile))
            {
                sw.Write("\n" + UUID);
            }
        }
        Debug.Log("Data being saved to: " + logFile);
    }

    public void LogData(bool logLineSize, bool logHeadPos)
    {
        // Write to the stored log file path
        using (StreamWriter sw = File.AppendText(logFile))
        {
            // Save the current size of the lines
            if (logLineSize) sw.Write("," + currentScale.ToString("F3") + "mm");
            // Also save head rotation position (horizontal + vertical)
            if (logHeadPos) sw.Write("," + xrCamera.localEulerAngles.y.ToString("F3") + "/" + xrCamera.localEulerAngles.x.ToString("F3"));
        }
    }

    public void Remove()
    {
        Destroy(lines);
    }
}