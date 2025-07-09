using LazyCoder.ScriptableObjects;
using UnityEngine;

namespace LazyCoder.SceneLoader
{
    public class SceneLoaderSos : ScriptableObjectSingleton<SceneLoaderSos>
    {
        [SerializeField] private GameObject _prefab;

        public static GameObject Prefab => Instance._prefab;
    }
}
