using UnityEngine;

namespace LazyCoder
{
    public static partial class ExtensionsGameObject
    {
        public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (component != null)
                Object.Destroy(component);
        }

        public static void DestroyChildren(this GameObject gameObject)
        {
            Transform transform = gameObject.transform;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyChildrenImmediate(this GameObject gameObject)
        {
            Transform transform = gameObject.transform;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public static void SetActiveChildren(this GameObject gameObject, bool isActive)
        {
            Transform transform = gameObject.transform;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }

        public static void SetLayerRecursively(this GameObject gameObject, int layerNumber)
        {
            Transform[] trans = gameObject.GetComponentsInChildren<Transform>();

            for (int i = 0; i < trans.Length; i++)
            {
                trans[i].gameObject.layer = layerNumber;
            }
        }

        public static Bounds GetRendererBounds(this GameObject go)
        {
            var hasBounds = false;
            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            var childrenRenderer = go.GetComponentsInChildren<Renderer>();

            var rnd = go.GetComponent<Renderer>();

            if (rnd != null)
            {
                bounds = rnd.bounds;
                hasBounds = true;
            }

            foreach (var child in childrenRenderer)
            {
                if (!hasBounds)
                {
                    bounds = child.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(child.bounds);
                }
            }

            return bounds;
        }

        #region Create

        public static T Create<T>(this T obj) where T : Object
        {
            return Object.Instantiate(obj);
        }

        public static T Create<T>(this T obj, Transform parent) where T : Object
        {
            return Object.Instantiate(obj, parent);
        }

        public static T Create<T>(this T gameObject, Transform parent, bool worldPositionStays) where T : Object
        {
            return Object.Instantiate(gameObject, parent, worldPositionStays);
        }

        public static T Create<T>(this T gameObject, Vector3 position, Quaternion? rotation = null) where T : Object
        {
            return Object.Instantiate(gameObject, position, rotation.GetValueOrDefault(Quaternion.identity));
        }

        public static T Create<T>(this T gameObject, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            return Object.Instantiate(gameObject, position, rotation, parent);
        }

        #endregion
    }
}