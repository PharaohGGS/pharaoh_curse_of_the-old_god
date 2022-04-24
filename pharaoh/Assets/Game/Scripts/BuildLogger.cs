using UnityEngine;
using System.Collections;

public class BuildLogger : MonoBehaviour
{
    public static BuildLogger Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string myLog;
    private Queue myLogQueue = new Queue();

    public int queueLimit = 25;
    public bool defaultLog = true;
    public bool warningLog = true;
    public bool errorLog = true;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Checks whether the log is wanted or not
        if (type == LogType.Log && defaultLog
            || type == LogType.Warning && warningLog
            || (type == LogType.Error || type == LogType.Exception) && errorLog)
        {
            myLog = logString;
            string newString = "\n [" + type + "]@" + System.DateTime.Now.ToLongTimeString() + " : " + myLog;
            myLogQueue.Enqueue(newString);

            if (myLogQueue.Count > queueLimit)
                myLogQueue.Dequeue();

            if (type == LogType.Exception)
            {
                newString = "\n" + stackTrace;
                myLogQueue.Enqueue(newString);
            }


            myLog = string.Empty;
            foreach (string mylog in myLogQueue)
            {
                myLog += mylog;
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label(myLog);
    }
}