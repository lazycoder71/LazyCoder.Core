using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LazyCoder.Core
{
    /// <summary>
    /// Class that contains methods useful for debugging.
    /// All methods are only compiled if the DEVELOPMENT_BUILD symbol or UNITY_EDITOR is defined.
    /// </summary>
    public static class LDebug
    {
        private const float HeaderColorStepStart = 0.5f;
        private const float HeaderColorStep = 0.075f;
        
        private static readonly Dictionary<string, Color> HeaderColorDict = new();

        private static int _headerColorCount = 0;

        #region Functions -> Public

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, Color? color = null)
        {
            Debug.Log(GetLog(message, color));
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object header, object message, Color? headerColor = null)
        {
            Debug.Log(GetLog(header, message, headerColor));
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log<T>(object message, Color? headerColor = null)
        {
            Debug.Log(GetLog(typeof(T), message, headerColor));
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, Color? color = null)
        {
            Debug.LogWarning(GetLog(message, color));
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object header, object message, Color? headerColor = null)
        {
            Debug.LogWarning(GetLog(header, message, headerColor));
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning<T>(object message, Color? headerColor = null)
        {
            Debug.LogWarning(GetLog(typeof(T), message, headerColor));
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, Color? color = null)
        {
            Debug.LogError(GetLog(message, color));
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object header, object message, Color? headerColor = null)
        {
            Debug.LogError(GetLog(header, message, headerColor));
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError<T>(object message, Color? headerColor = null)
        {
            Debug.LogError(GetLog(typeof(T), message, headerColor));
        }

        #endregion

        #region Functions -> Private

        private static object GetLog(object message, Color? color)
        {
            return color.HasValue ? $"<color=#{ColorUtility.ToHtmlStringRGB(color.Value)}>{message}</color>" : message;
        }

        private static object GetLog(object header, object message, Color? headerColor)
        {
            Color color;

            if (headerColor.HasValue)
            {
                color = headerColor.Value;
            }
            else
            {
                if (HeaderColorDict.ContainsKey(header.ToString()))
                {
                    color = HeaderColorDict[header.ToString()];
                }
                else
                {
                    // Lerp rainbow color
                    color = Color.HSVToRGB(
                        Mathf.PingPong(HeaderColorStepStart + _headerColorCount * HeaderColorStep, 1), 1, 1);

                    HeaderColorDict.Add(header.ToString(), color);

                    _headerColorCount++;
                }
            }

            return $"[<color=#{ColorUtility.ToHtmlStringRGB(color)}>{header}</color>] {message}";
        }

        #endregion
    }
}