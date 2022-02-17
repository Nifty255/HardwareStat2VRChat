using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    // UI handling
    public ScrollRect scrollRect;
    public Text textArea;
    public bool scrollDown = true;

    // Text handling
    string logText = "Starting up!!! Close the window to exit.";
    int maxChars = 65000;

    // Event handling
    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }

    public void Log(string logString, string stackTrace, LogType type)
    {
        // Append the log string to the text. If it's too long, truncate it to the last `maxChars`.
        logText = logText + "\n" + logString;
        if (logText.Length > maxChars) { logText = logText.Substring(logText.Length - maxChars); }

        // Change the UI and, if auto-scroll is enabled, tell the UI to do so at the end of the frame.
        textArea.text = logText;
        if (scrollDown)
        {
            StartCoroutine(ApplyScrollPosition());
        }
    }

    IEnumerator ApplyScrollPosition()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scrollRect.transform);
    }
}