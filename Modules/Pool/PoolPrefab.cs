using UnityEngine;
using UnityEngine.Pool;

namespace LFramework.Pool
{
    public class PoolPrefab : ObjectPool<GameObject>
    {
        public PoolPrefab(GameObject prefab) : base(
            () => { return Object.Instantiate(prefab); },
            (obj) => { if (obj != null) obj.SetActive(true); },
            (obj) => { if (obj != null) obj.SetActive(false); },
            (obj) => { },
#if UNITY_EDITOR
            true) // Keep heavy check on editor
#else
            false)
#endif
        {
        }
    }
}
