using System.IO;
using UnityEngine;

public class LogController
{
    // Data to be logged:
    private string UUID;
    private int eyeLeft, eyeRight, glasses;
    // Store all final line sizes
    // Horizontal Inf, Vertical Inf, Diagonal Inf, Horizontal E, Vertical E, Diagonal E
    private float[] staticLines = {-1,-1,-1,-1,-1,-1};
    private float[] dynamicLines = {-1,-1,-1,-1,-1,-1};

    public void Init(string userUUID)
    {
        if (!File.Exists(Application.persistentDataPath + "/" + Constants.LOGFILE))
        {
            // Set up the CSV
            using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + Constants.LOGFILE))
            {
                // Write the header line
                sw.WriteLine("UUID,Eye Left,Eye Right,Glasses," +
                    "Static Line Horizontal,Static Line Vertical,Static Line Diagonal," +
                    "Static E Horizontal,Static E Vertical,Static E Diagonal," +
                    "Dynamic Line Hoizontal,Dynamic Line Vertical,Dynamic Line Diagonal" +
                    "Dynamic E Hoizontal,Dynamic E Vertical,Dynamic E Diagonal");
            }
        }
        // Save the UUID for later writing
        UUID = userUUID;
        Debug.Log("Data being saved to: " + Application.persistentDataPath + "/" + Constants.LOGFILE);
    }

    public void LogUserData(int leftEye, int rightEye, int hasGlasses)
    {
        // Store passed data into the logger
        eyeLeft = leftEye;
        eyeRight = rightEye;
        glasses = hasGlasses;
    }

    public void LogLineData(float lineScale, Constants.LINE_TYPE LT, Constants.LINE_ORIENTATION LO, bool isDynamic = false)
    {
        if (isDynamic)
        {
            // 0-2 should be infinite lines, 3-5 should be E
            dynamicLines[(int)LT * 3 + (int)LO] = lineScale;
        }
        else
        {
            staticLines[(int)LT * 3 + (int)LO] = lineScale;
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
                "," + glasses);
            // STATIC TESTING
            for (int i = 0; i < staticLines.Length; i++)
            {
                sw.Write("," + staticLines[i].ToString("F3") + "mm");
            }
            // DYNAMIC TESTING
            for (int i = 0; i < dynamicLines.Length; i++)
            {
                sw.Write("," + dynamicLines[i].ToString("F3") + "mm");
            }
            sw.Write("\n");
        }    
    }
}