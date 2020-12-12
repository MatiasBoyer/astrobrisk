using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ConsoleViewer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI TMP_Logger;

    [Header("Console Colors")]
    public Color C_Default;
    public Color C_Warning;
    public Color C_Error;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logstring, string stacktrace, LogType type)
    {
        AddMessageToConsole(logstring, type);
    }

    public void AddMessageToConsole(string log, LogType type)
    {
        Color selectedColor = new Color();
        switch(type)
        {
            case LogType.Warning:
                selectedColor = C_Warning;
                break;
            case LogType.Error:
                selectedColor = C_Error;
                break;
            default:
                selectedColor = C_Default;
                break;
        }

        string c = string.Format("<color=#{0}>[{1}] {2}</color>\n", ColorUtility.ToHtmlStringRGB(selectedColor), DateTime.Now.ToString("hh:mm:ss"), log);
        TMP_Logger.text += c;
    }

}
