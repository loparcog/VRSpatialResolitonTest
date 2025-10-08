using System.IO;
using UnityEngine;

public class LogController
{
    // Data to be logged:
    private string UUID;
    private int eyeLeft, eyeRight, IPD, glasses;
    // S = Static, D = Dynamic
    // H = Horizontal, V = Vertical, D = Diagonal
    private float SHLine, SVLine, SDLine, DHLine, DVLine, DDLine;
    // Head position and rotation for each line
    private Vector3 HPH, HPV, HPD, HRH, HRV, HRD;

    public void Init(string userUUID)
    {
        if (!File.Exists(Application.persistentDataPath + "/" + Constants.LOGFILE))
        {
            // Set up the CSV
            using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + Constants.LOGFILE))
            {
                // UUID for the user
                sw.WriteLine("UUID,Eye Left,Eye Right,IPD,Glasses," +
                    "Static Horizontal,Static Vertical,Static Diagonal," +
                    "Dynamic Hoizontal,Head Rotation Horizontal,Head Position Horizontal," +
                    "Dynamic Vertical,Head Rotation Vertical,Head Position Vertical," +
                    "Dynamic Diagonal,Head Rotation Diagonal,Head Position Diagonal");
                // Write the user's UUID
            }
        }
        UUID = userUUID;
        Debug.Log("Data being saved to: " + Application.persistentDataPath + "/" + Constants.LOGFILE);
    }

    public void LogUserData(int leftEye, int rightEye, int IPDist, int hasGlasses)
    {
        // Store passed data into the logger
        eyeLeft = leftEye;
        eyeRight = rightEye;
        IPD = IPDist;
        glasses = hasGlasses;
    }

    public void LogLineData(float lineScale, Constants.LINE_ORIENTATION LO, Transform xrCamera = null)
    {
        if (xrCamera != null)
        {
            // For dynamic settings
            switch (LO)
            {
                case Constants.LINE_ORIENTATION.HORIZONTAL:
                    DHLine = lineScale;
                    HPH = xrCamera.localPosition;
                    HRH = xrCamera.localEulerAngles;
                    break;
                case Constants.LINE_ORIENTATION.VERTICAL:
                    DVLine = lineScale;
                    HPV = xrCamera.localPosition;
                    HRV = xrCamera.localEulerAngles;
                    break;
                case Constants.LINE_ORIENTATION.DIAGONAL:
                    DDLine = lineScale;
                    HPD = xrCamera.localPosition;
                    HRD = xrCamera.localEulerAngles;
                    break;
            }
            Debug.Log("LOOK: " + HPH + ", " + HPV + ", " + HPD);
        }
        else
        {
            // For static settings
            switch (LO)
            {
                case Constants.LINE_ORIENTATION.HORIZONTAL:
                    SHLine = lineScale;
                    break;
                case Constants.LINE_ORIENTATION.VERTICAL:
                    SVLine = lineScale;
                    break;
                case Constants.LINE_ORIENTATION.DIAGONAL:
                    SDLine = lineScale;
                    break;
            }
        }
    }

    public void WriteToFile()
    {
        // Write to the stored log file path
        using (StreamWriter sw = File.AppendText(Application.persistentDataPath + "/" + Constants.LOGFILE))
        {
            // Write all formatted data to the line
            // EYE DATA
            sw.Write(UUID + ",20/" + eyeLeft +
                ",20/" + eyeRight +
                "," + IPD + "mm" +
                "," + glasses);
            // STATIC TESTING
            sw.Write("," + SHLine.ToString("F3") + "mm" +
                "," + SVLine.ToString("F3") + "mm" +
                "," + SDLine.ToString("F3") + "mm");
            // DYNAMIC TESTING
            sw.Write("," + DHLine.ToString("F3") + "mm" +
                "," + HRH.y.ToString("F3") + "/" + HRH.x.ToString("F3") +
                "," + HPH.x.ToString("F3") + "/" + HPH.y.ToString("F3") + "/" + HPH.z.ToString("F3") +
                "," + DVLine.ToString("F3") + "mm" +
                "," + HRV.y.ToString("F3") + "/" + HRV.x.ToString("F3") +
                "," + HPV.x.ToString("F3") + "/" + HPV.y.ToString("F3") + "/" + HPV.z.ToString("F3") +
                "," + DDLine.ToString("F3") + "mm" +
                "," + HRD.y.ToString("F3") + "/" + HRD.x.ToString("F3") +
                "," + HPD.x.ToString("F3") + "/" + HPD.y.ToString("F3") + "/" + HPD.z.ToString("F3") + "\n");
        }    
    }
}