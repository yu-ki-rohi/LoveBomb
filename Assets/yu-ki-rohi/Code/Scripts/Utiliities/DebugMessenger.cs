using System.Runtime.CompilerServices;
using UnityEngine;

public static class DebugMessenger
{
    public static bool NullCheckError<T>(T instance, string additionalMessage = "", [CallerFilePath] string filePath = "",  [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "") where T : class
    {
        if(instance == null)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            Debug.LogError(typeof(T).Name + " is Null!! " + additionalMessage + "\n[ " + fileName + " : " + lineNumber + " ]"); 
            return true;
        }

        return false;
    }

    public static bool NullCheckWarning<T>(T instance, string additionalMessage = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "") where T : class
    {
        if (instance == null)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            Debug.LogWarning(typeof(T).Name + " is Null!! " + additionalMessage + "\n[ " + fileName + " : " + lineNumber + " ]");
            return true;
        }

        return false;
    }
    public static bool NullCheck<T>(T instance, string additionalMessage = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "") where T : class
    {
        if (instance == null)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            Debug.Log(typeof(T).Name + " is Null!! " + additionalMessage + "\n[ " + fileName + " : " + lineNumber + " ]");
            return true;
        }

        return false;
    }
}
