using System.Collections.Generic;
using UnityEngine;

namespace LazyCoder.Pool
{
    internal static class PoolCallbackHelper
    {
        private static readonly List<IPoolCallbackReceiver> s_componentsBuffer = new();

        public static void InvokeOnGet(GameObject obj)
        {
            obj.GetComponentsInChildren(s_componentsBuffer);

            for (int i = 0; i < s_componentsBuffer.Count; i++)
            {
                s_componentsBuffer[i].OnGet();
            }
        }

        public static void InvokeOnRelease(GameObject obj)
        {
            obj.GetComponentsInChildren(s_componentsBuffer);

            for (int i = 0; i < s_componentsBuffer.Count; i++)
            {
                s_componentsBuffer[i].OnRelease();
            }
        }
    }
}