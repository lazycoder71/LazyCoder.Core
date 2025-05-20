using System;
using System.Collections.Generic;
using UnityEngine;

namespace LFramework.Pool
{
    public static class PoolPrefabShared
    {
        private static Dictionary<GameObject, PoolPrefab> s_poolByPrefab = new();
        private static Dictionary<GameObject, PoolPrefab> s_poolByInstance = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            s_poolByPrefab.Clear();
            s_poolByInstance.Clear();
        }

        public static T Get<T>(GameObject prefab) where T : class
        {
            return Get(prefab).GetComponent<T>();
        }

        public static GameObject Get(GameObject prefab, Transform parent)
        {
            GameObject result = Get(prefab);

            result.transform.SetParent(parent);

            return result;
        }

        public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject result = Get(prefab);

            result.transform.parent = parent;
            result.transform.position = position;
            result.transform.rotation = rotation;

            return result;
        }

        public static GameObject Get(GameObject prefab)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            PoolPrefab pool = GetOrCreatePool(prefab);

            GameObject instance = null;

            while (instance == null)
                instance = pool.Get();

            s_poolByInstance.Add(instance, pool);

            PoolCallbackHelper.InvokeOnGet(instance);

            return instance;
        }

        public static void Release(GameObject instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            s_poolByInstance.TryGetValue(instance, out PoolPrefab pool);

            if (pool == null)
                return;

            pool.Release(instance);

            PoolCallbackHelper.InvokeOnRelease(instance);

            s_poolByInstance.Remove(instance);
        }

        public static void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            PoolPrefab pool = GetOrCreatePool(prefab);

            GameObject[] instances = new GameObject[count];

            for (int i = 0; i < count; i++)
                instances[i] = pool.Get();

            for (int i = 0; i < count; i++)
                pool.Release(instances[i]);
        }

        public static PoolPrefab GetOrCreatePool(GameObject prefab)
        {
            if (!s_poolByPrefab.ContainsKey(prefab))
                s_poolByPrefab.Add(prefab, new PoolPrefab(prefab));

            return s_poolByPrefab[prefab];
        }

        public static PoolPrefab GetPool(GameObject prefab)
        {
            s_poolByPrefab.TryGetValue(prefab, out var pool);

            return pool;
        }

        public static void ClearPool(GameObject prefab)
        {
            PoolPrefab pool = GetPool(prefab);

            pool.Clear();
            pool.Dispose();
        }
    }
}