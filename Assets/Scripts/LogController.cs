using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
    Data Logging Controller
*/
public class LogController
{
    // UUID for user
    private string UUID;
    // Debug flag
    private bool debug;
    private int eyeLeft, eyeRight, glasses;
    // Store all final line sizes
    // Horizontal Inf, Vertical Inf, Diagonal Inf, Horizontal E, Vertical E, Diagonal E
    private float[] staticLines = {-1,-1,-1,-1,-1,-1};
    private float[] dynamicLines = {-1,-1,-1,-1,-1,-1};

    public void Init(string userUUID, bool printDebug)
    {
        if (!File.Exists(Application.persistentDataPath + "/" + Constants.LOGFILE))
        {
            // Set up CSV headers using tests chosen in Constants.cs
            string testHeaders = "";
            foreach (string testType in new List<string> {"STATIC", "DYNAMIC"})
            {
                foreach (string lineType in Enum.GetNames(typeof(Constants.LINE_TYPE)))
                {
                    foreach (string lineOri in Enum.GetNames(typeof(Constants.LINE_ORIENTATION)))
                    {
                        testHeaders += "," + string.Join(" ", testType, lineType, lineOri);
                    }
                    
                }
            }

            Debug.Log(testHeaders);

            using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + Constants.LOGFILE))
            {
                // User data prefix
                sw.WriteLine("UUID,EYE LEFT,EYE RIGHT,GLASSES" + testHeaders);
            }
        }
        // Save the UUID for later writing
        UUID = userUUID;
        debug = printDebug;
        // Note whether debug should be printed
        Debug.Log("Data being saved to: " + Application.persistentDataPath + "/" + Constants.LOGFILE);
    }

    public void LogUserData(int leftEye, int rightEye, int hasGlasses)
    {
        // Store passed data into the logger
        eyeLeft = leftEye;
        eyeRight = rightEye;
        glasses = hasGlasses;

        if (debug) { Debug.Log(string.Join(" ", "ELeft:", eyeLeft, "ERight:", eyeRight, "Glasses:", glasses)); }
    }

    public void LogLineData(float lineScale, Constants.LINE_TYPE LT, Constants.LINE_ORIENTATION LO, bool isDynamic = false)
    {
        // Store the tests in the order they were taken
        if (isDynamic)
        {
            dynamicLines[(int)LT * 3 + (int)LO] = lineScale;
        }
        else
        {
            staticLines[(int)LT * 3 + (int)LO] = lineScale;
        }

        if (debug) { Debug.Log(string.Join(" ", isDynamic ? "Dynamic" : "Static", LT.ToString(), LO.ToString(), ":", lineScale)); }
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