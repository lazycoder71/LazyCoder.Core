using UnityEngine;

namespace LazyCoder.ScriptableObjects
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static readonly string s_rootFolderName = "ScriptableObjectSingleton";

        private static T s_instance = null;

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = Resources.Load<T>($"{s_rootFolderName}/{typeof(T)}");

#if UNITY_EDITOR
                    if (s_instance == null)
                    {
                        string configPath = $"Assets/_ROOT/Resources/{s_rootFolderName}/";

                        if (!System.IO.Directory.Exists(configPath))
                            System.IO.Directory.CreateDirectory(configPath);

                        s_instance = ScriptableObjectHelper.CreateAsset<T>(configPath, typeof(T).ToString());
                    }
#endif
                }

                return s_instance;
            }
        }
    }
}