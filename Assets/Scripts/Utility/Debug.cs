using System;
using UnityEngine;

public static class Debug
{
    public static bool isDebugBuild
    {
        get { return UnityEngine.Debug.isDebugBuild; }
    }

    static string GetTime()
    {
        return $"<color=blue>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}</color> ";
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message)
    {
#if ENABLE_LOG
        UnityEngine.Debug.Log(GetTime() + message);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message, UnityEngine.Object context)
    {
#if ENABLE_LOG
        UnityEngine.Debug.Log(GetTime() + message, context);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogFormat(string format, params object[] args)
    {
#if ENABLE_LOG
        UnityEngine.Debug.LogFormat(GetTime() + format, args);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object message)
    {
#if ENABLE_LOG
        UnityEngine.Debug.LogError(GetTime() + message);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object message, UnityEngine.Object context)
    {
#if ENABLE_LOG
        UnityEngine.Debug.LogError(GetTime() + message, context);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object message)
    {
#if ENABLE_LOG
        UnityEngine.Debug.LogWarning(GetTime() + message.ToString());
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object message, UnityEngine.Object context)
    {
#if ENABLE_LOG
        UnityEngine.Debug.LogWarning(GetTime() + message.ToString(), context);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
#if ENABLE_LOG
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
#if ENABLE_LOG
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Assert(bool condition, string message = "")
    {
#if ENABLE_LOG
        if (!condition) throw new Exception(message);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogException(Exception exception)
    {
#if ENABLE_LOG
        UnityEngine.Debug.LogException(exception);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarningFormat(string format, params object[] args)
    {
#if ENABLE_LOG
        UnityEngine.Debug.LogWarningFormat(format, args);
#endif
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogErrorFormat(string format, params object[] args)
    {
#if ENABLE_LOG
        UnityEngine.Debug.LogErrorFormat(format, args);
#endif
    }
}

