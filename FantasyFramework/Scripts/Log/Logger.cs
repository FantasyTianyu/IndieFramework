using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 日志系统
/// </summary>
public class Logger
{
    //日志级别
    private static LogLevel level = LogLevel.Debug;

    /// <summary>
    /// Sets the log level.
    /// </summary>
    /// <param name="_level">Level.</param>
    public static void SetLogLevel(LogLevel _level)
    {
        level = _level;
    }

    public static void DebugLog(object message)
    {
        if (level <= LogLevel.Debug)
            Debug.Log("[" + System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " Debug " + message + "]");
    }

    public static void Info(object message)
    {
        if (level <= LogLevel.Info)
            Debug.Log("[" + System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " Info " + message + "]");
    }

    public static void Warning(object message)
    {
        if (level <= LogLevel.Warning)
            Debug.LogWarning("[" + System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " Warning " + message + "]");
    }

    public static void Error(object message)
    {
        if (level <= LogLevel.Warning)
            Debug.LogError("[" + System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " Error " + message + "]");
    }
}
