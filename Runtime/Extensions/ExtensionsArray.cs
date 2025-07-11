using System;
using System.Security.Cryptography;
using UnityEngine;

namespace LazyCoder.Core
{
    public static class ExtensionsArray
    {
        public static void Shuffle<T>(this T[] arr)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = arr.Length;
            while (n > 1)
            {
                byte[] box = new byte[1];

                do
                {
                    provider.GetBytes(box);
                }
                while (!(box[0] < n * (Byte.MaxValue / n)));

                int k = (box[0] % n);
                n--;
                T value = arr[k];
                arr[k] = arr[n];
                arr[n] = value;
            }
        }

        public static T First<T>(this T[] array)
        {
            return array[0];
        }

        public static T Last<T>(this T[] array)
        {
            return array[array.Length - 1];
        }

        public static T GetClamp<T>(this T[] array, int index)
        {
            return array[Mathf.Clamp(index, 0, array.Length - 1)];
        }

        public static T GetRandom<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static T GetSafe<T>(this T[] array, int index)
        {
            if (index < 0 || index >= array.Length)
                return default;

            return array[index];
        }

        public static void Swap<T>(this T[] data, int index0, int index1)
        {
            T t = data[index0];
            data[index0] = data[index1];
            data[index1] = t;
        }

        public static T GetLoop<T>(this T[] array, int index)
        {
            return array[array.GetLoopIndex(index)];
        }

        public static void SetLoop<T>(this T[] array, int index, T item)
        {
            array[array.GetLoopIndex(index)] = item;
        }

        public static int GetLoopIndex<T>(this T[] array, int index)
        {
            if (index < 0)
                return (array.Length - (Mathf.Abs(index) % array.Length)) % array.Length;
            else
                return index % array.Length;
        }
    }
}
