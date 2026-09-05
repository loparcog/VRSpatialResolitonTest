using System.IO;
using UnityEngine;


/*
    Line Pair Utility Class
    Used for all instantiation and modification of a given line
    pair object. All line types can be found in Assets/Resources/Lines,
    and tested lines can be modified in Constants.cs
*/
public class LinePair : MonoBehaviour
{
    // Lines to instantiate
    public GameObject lines;
    // Camera reference for dynamic testing, maintain line distance
    private Transform xrCamera;
    // Current scale, accessible for writing to logs
    public float currentScale = 0.5f;
    // Save the line type, as E requires x/y scaling, while others only require y
    private string currentLineType;
    // Max line size
    const float LINE_MAX = 1.0f;

    public void SetCamera(Transform camera)
    {
        // Save camera for head rotation
        xrCamera = camera;
    }

    public void MakeLines(string lineType, float scale = LINE_MAX)
    {
        /*
            Instantiate the selected lineType
            Default = Five 1cm line pairs
            Box = Five capped 1cm line pairs
            Infinite = Five infinitely long 1cm line pairs
            E = A single E shape with 1cm lines
        */
        lines = Instantiate(Resources.Load<GameObject>("Lines/" + lineType));
        lines.name = "Line Pairs";
        // Reset the current scale
        currentScale = scale;
        // Scale the scene to match the scale point
        UpdateSize();
    }

    public void RotateTo(float angle)
    {
        // Make sure lines exist
        if (lines == null) return;
        lines.transform.rotation = Quaternion.Euler(0, angle, 0);
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
        if (currentLineType == "HLP E")
        {
            // Scale width and length
            lines.transform.localScale = new Vector3(currentScale, 1, currentScale);
        }
        else
        {
            // Scale only width, keep lines infinite
            lines.transform.localScale = new Vector3(1, 1, currentScale);
        }
    }

    public void keepDistance()
    {
        // Maintain distance between camera and lines (0.5cm)
        if (lines == null) return;
        // Requires SetCamera() to be run beforehand
        lines.transform.position = new Vector3(0, -xrCamera.localPosition.z, 0);
    
    }

    public void Remove()
    {
        Destroy(lines);
    }
}