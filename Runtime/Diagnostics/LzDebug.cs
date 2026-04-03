using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LazyCoder.Core
{
    /// <summary>
    /// Provides utility methods for logging debug messages with optional color and headers.
    /// Only compiled if DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
    /// </summary>
    public static class LzDebug
    {
        private static readonly Color[] HeaderColors =
        {
            Color.softRed,
            Color.orange,
            Color.yellowNice,
            Color.limeGreen,
            Color.deepSkyBlue,
            Color.mediumPurple,
            Color.hotPink,
            Color.cyan,
            Color.white,
        };

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

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogException(System.Exception exception, Object context = null)
        {
            Debug.LogException(exception, context);
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
            var headerKey = header?.ToString() ?? "null";

            if (headerColor.HasValue)
            {
                color = headerColor.Value;
            }
            else
            {
                if (HeaderColorDict.TryGetValue(headerKey, out var cachedColor))
                {
                    color = cachedColor;
                }
                else
                {
                    // Cycle through a deterministic rainbow palette for new headers.
                    color = HeaderColors[_headerColorCount % HeaderColors.Length];

                    HeaderColorDict.Add(headerKey, color);

                    _headerColorCount++;
                }
            }

            return $"[<color=#{ColorUtility.ToHtmlStringRGB(color)}>{header}</color>] {message}";
        }

        #endregion
    }
}