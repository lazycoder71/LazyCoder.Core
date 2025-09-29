using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LazyCoder.Core
{
    public static class ExtensionsList
    {
        public static void Resize<T>(this List<T> list, int newSize, T defaultValue = default(T))
        {
            int currentSize = list.Count;
            if (newSize < currentSize)
            {
                list.RemoveRange(newSize, currentSize - newSize);
            }
            else if (newSize > currentSize)
            {
                if (newSize > list.Capacity)
                    list.Capacity = newSize;

                list.AddRange(Enumerable.Repeat(defaultValue, newSize - currentSize));
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            System.Random random = new System.Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static void Swap<T>(this List<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }

        public static bool IsOutOfBounds<T>(this IList<T> list, int index)
        {
            if (list == null)
                return true;

            return index < 0 || index >= list.Count;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static List<T> Clone<T>(this List<T> list) where T : ICloneable
        {
            return list.Select(item => (T)item.Clone()).ToList();
        }

        public static T Last<T>(this List<T> list)
        {
            return list[^1];
        }

        public static T First<T>(this List<T> list)
        {
            return list[0];
        }

        public static T GetClamp<T>(this List<T> list, int index)
        {
            return list[Mathf.Clamp(index, 0, list.Count - 1)];
        }

        public static T GetRandom<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T GetLoop<T>(this List<T> list, int index)
        {
            return list[list.GetLoopIndex(index)];
        }

        public static void SetLoop<T>(this List<T> list, int index, T item)
        {
            list[list.GetLoopIndex(index)] = item;
        }

        public static int GetLoopIndex<T>(this List<T> list, int index)
        {
            if (index < 0)
                return (list.Count - (Mathf.Abs(index) % list.Count)) % list.Count;
            else
                return index % list.Count;
        }

        public static T Pop<T>(this List<T> list) where T : class
        {
            if (list.Count <= 0)
                return null;

            T result = list.Last();
            list.RemoveAt(list.Count - 1);
            return result;
        }
    }
}