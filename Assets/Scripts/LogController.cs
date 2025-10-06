using System.IO;
using UnityEngine;

public class LogController
{
    public static void Init(string UUID)
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
                sw.Write(UUID);
            }
        }
        else
        {
            // Just write the UUID as a new line in the file
            using (StreamWriter sw = File.AppendText(Application.persistentDataPath + "/" + Constants.LOGFILE))
            {
                sw.Write("\n" + UUID);
            }
        }
        Debug.Log("Data being saved to: " + Application.persistentDataPath + "/" + Constants.LOGFILE);
    }

    public static void LogUserData(int leftEye, int rightEye, int IPD, int glasses)
    {
        // Write to the stored log file path
        using (StreamWriter sw = File.AppendText(Application.persistentDataPath + "/" + Constants.LOGFILE))
        {
            // Write all formatted data to the line
            sw.Write(",20/" + leftEye +
                ",20/" + rightEye +
                "," + IPD + "mm" +
                "," + glasses);
        }
    }

    public static void LogLineData(float lineScale, Transform xrCamera = null)
    {
        // Write to the stored log file path
        using (StreamWriter sw = File.AppendText(Application.persistentDataPath + "/" + Constants.LOGFILE))
        {
            // Save the current size of the lines
            sw.Write("," + lineScale.ToString("F3") + "mm");
            // Also save head rotation (horizontal/vertical) and position (X/Y/Z)
            if (xrCamera != null)
            {
                sw.Write("," + xrCamera.localEulerAngles.y.ToString("F3") + "/" + xrCamera.localEulerAngles.x.ToString("F3"));
                sw.Write("," + xrCamera.localPosition.x.ToString("F3") + "/" + xrCamera.localPosition.y.ToString("F3") + "/" + xrCamera.localPosition.z.ToString("F3"));
            }
        }
    }
}