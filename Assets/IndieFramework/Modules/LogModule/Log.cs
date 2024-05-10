using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieFramework {
    public enum LogLevel {
        Info,
        Warning,
        Error,
    }

    public class Log {
        public static LogLevel CurrentLogLevel = LogLevel.Info;

        private static void ShowLog(LogLevel level, object message) {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}]: {message}";
            if (level >= CurrentLogLevel) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                switch (level) {
                    case LogLevel.Info:
                        Debug.Log(logMessage);
                        break;
                    case LogLevel.Warning:
                        Debug.LogWarning(logMessage);
                        break;
                    case LogLevel.Error:
                        Debug.LogError(logMessage);
                        break;
                }
#endif
            }
        }

        public static void LogInfo(object message) {
            ShowLog(LogLevel.Info, message);
        }

        public static void LogWarning(object message) {
            ShowLog(LogLevel.Warning, message);
        }

        public static void LogError(object message) {
            ShowLog(LogLevel.Error, message);
        }

    }
}