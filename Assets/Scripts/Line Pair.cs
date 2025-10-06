using System.IO;
using UnityEngine;

public class LinePair : MonoBehaviour
{
    // Horizontal line pair (1mm lines)
    public GameObject lines;
    private Transform xrCamera;
    // Current scale, accessible for writing to logs
    public float currentScale = 0.5f;

    const float LINE_MAX = 1.0f;

    public void SetCamera(Transform camera)
    {
        // Save camera for head rotation
        xrCamera = camera;
    }

    public void MakeLines(string lineType, float scale = LINE_MAX)
    {
        // Instantiate the line pair
        /*
            HLP = Standard
            HLP Box = Capped ends
            HLP Infinite = Infinitely long lines
        */
        lines = Instantiate(Resources.Load<GameObject>(lineType));
        lines.name = "Line Pairs";
        // Reset the current scale
        currentScale = scale;
        // Scale the scene to match the scale point
        UpdateSize();
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
        UpdateSize();
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
        UpdateSize();
    }

    public void UpdateSize()
    {
        lines.transform.localScale = new Vector3(1, 1, currentScale); // FOR WIDTH RESIZING ONLY
        //lines.transform.localScale = new Vector3(currentScale, 1, currentScale); // FOR WIDTH AND LENGTH RESIZING
    }

    public void keepDistance()
    {
        if (lines)
        {
            // Requires SetCamera() to be run beforehand
            lines.transform.position = new Vector3(0, -xrCamera.localPosition.z, 0);
        }
    }

    public void Remove()
    {
        Destroy(lines);
    }
}