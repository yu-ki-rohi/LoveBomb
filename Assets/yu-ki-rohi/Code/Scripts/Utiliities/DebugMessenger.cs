using System.Runtime.CompilerServices;
using UnityEngine;

public static class DebugMessenger
{
    private static bool enableMessage = true;
    public static bool NullCheckError<T>(T instance, string additionalMessage = "", [CallerFilePath] string filePath = "",  [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "") where T : class
    {
        if(instance == null)
        {
            if (enableMessage == false) { return true; }
            string fileName = System.IO.Path.GetFileName(filePath);
            Debug.LogError("[ " + fileName + " : " + lineNumber + " ] " + typeof(T).Name + " is Null!! " + additionalMessage); 
            return true;
        }

        return false;
    }

    public static bool NullCheckWarning<T>(T instance, string additionalMessage = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "") where T : class
    {
        if (instance == null)
        {
            if (enableMessage == false) { return true; }
            string fileName = System.IO.Path.GetFileName(filePath);
            Debug.LogWarning("[ " + fileName + " : " + lineNumber + " ] " + typeof(T).Name + " is Null!! " + additionalMessage);
            return true;
        }

        return false;
    }
    public static bool NullCheck<T>(T instance, string additionalMessage = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "") where T : class
    {
        if (instance == null)
        {
            if (enableMessage == false) { return true; }
            string fileName = System.IO.Path.GetFileName(filePath);
            Debug.Log( " [ " + fileName + " : " + lineNumber + " ] " + typeof(T).Name + " is Null!! " + additionalMessage);
            return true;
        }

        return false;
    }

    public static void Log(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
    {
        if (enableMessage == false) { return; }
        string fileName = System.IO.Path.GetFileName(filePath);
        Debug.Log("[ " + fileName + " : " + lineNumber + " ] " + message);
    }
}
