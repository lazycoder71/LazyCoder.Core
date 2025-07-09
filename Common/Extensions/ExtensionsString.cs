using System.Runtime.CompilerServices;
using UnityEngine;

namespace LazyCoder
{
    public static class ExtensionsString
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SetColor(this string text, string color) { return $"<color=#{color}>{text}</color>"; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SetColor(this string text, UnityEngine.Color color) { return $"<color={ToHtmlStringRGBA(color)}>{text}</color>"; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SetSize(this string text, int size) { return $"<size={size}>{text}</size>"; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBold(this string text) { return $"<b>{text}</b>"; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToItalic(this string text) { return $"<i>{text}</i>"; }

        private static string ToHtmlStringRGBA(Color color)
        {
            var color32 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * byte.MaxValue), 0, byte.MaxValue),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * byte.MaxValue), 0, byte.MaxValue),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * byte.MaxValue), 0, byte.MaxValue),
                (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * byte.MaxValue), 0, byte.MaxValue));

            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color32.r, color32.g, color32.b, color32.a);
        }
    }
}