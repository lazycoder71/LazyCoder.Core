using UnityEngine;

namespace LazyCoder.Core
{
    public static class ExtensionsVector2Int 
    {
        public static int RandomWithin(this Vector2Int v)
        {
            return Random.Range(v.x, v.y);
        }
    }
}