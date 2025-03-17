using LFramework.ScriptableObjects;
using UnityEngine;

namespace LFramework.SceneLoader
{
    public class SceneLoaderSos : ScriptableObjectSingleton<SceneLoaderSos>
    {
        [SerializeField] private GameObject _prefab;

        public static GameObject Prefab => Instance._prefab;
    }
}
