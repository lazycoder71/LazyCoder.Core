using UnityEngine;

namespace LazyCoder.Core
{
    public static class ExtensionsBounds
    {
        public static Vector3 GetRandomPointInside(this Bounds bounds)
        {
            return new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));
        }
    }
}