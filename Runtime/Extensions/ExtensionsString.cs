using System.Runtime.CompilerServices;
using UnityEngine;

namespace LazyCoder.Core
{
    public static class ExtensionsString
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SetColor(this string text, string color)
        {
            return $"<color=#{color}>{text}</color>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SetColor(this string text, Color color)
        {
            return $"<color={ToHtmlStringRgba(color)}>{text}</color>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SetSize(this string text, int size)
        {
            return $"<size={size}>{text}</size>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBold(this string text)
        {
            return $"<b>{text}</b>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToItalic(this string text)
        {
            return $"<i>{text}</i>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ToHtmlStringRgba(Color color)
        {
            var color32 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * byte.MaxValue), 0, byte.MaxValue),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * byte.MaxValue), 0, byte.MaxValue),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * byte.MaxValue), 0, byte.MaxValue),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * byte.MaxValue), 0, byte.MaxValue));

            return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
        }
    }
}