using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPrint
{
    #region UNITY_EDITOR
    public static void EditorPrint(object message)
    {
        return;
        Debug.Log(message);
    }

    public static void ChatEditorPrint(object message)
    {
        return;
        Debug.Log(message);
    }

    public static void Print(object message)
    {
        return;
        Debug.Log(message);
    }

    public static void Print(object message, string color)
    {
        return;
        Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), message));
    }
    public static void PrintError(object message)
    {
        return;
        Debug.LogError(message);
    }

    public static void PrintWarning(object message)
    {
        return;
        Debug.LogWarning(message);
    }

    public static void xErrorNotice(object message)
    {
        return;
        Debug.LogError(message);
    }
    #endregion
}
