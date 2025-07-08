#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LFramework.ScriptableObjects
{
    public static class ScriptableObjectHelper
    {
        public static T CreateAsset<T>(string path, string fileName) where T : ScriptableObject
        {
            string filePath = $"{path}/{fileName}.asset";

            if (AssetDatabase.LoadAssetAtPath<T>(filePath))
            {
                LDebug.Log(typeof(ScriptableObjectHelper), $"File {filePath} is already exist");
                return null;
            }

            T asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, filePath);
            SaveAssetsDatabase();

            return AssetDatabase.LoadAssetAtPath(filePath, typeof(T)) as T;
        }

        public static void SaveAsset(ScriptableObject asset)
        {
            EditorUtility.SetDirty(asset);
            SaveAssetsDatabase();
        }

        public static T LoadOrCreateNewAsset<T>(string path, string fileName) where T : ScriptableObject
        {
            string filePath = $"{path}/{fileName}.asset";

            T asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(T)) as T;

            if (asset == null)
                asset = CreateAsset<T>(path, fileName);

            return asset;
        }

        public static T CopyAsset<T>(string oldPath, string newPath) where T : ScriptableObject
        {
            DeleteAsset<T>(newPath);

            AssetDatabase.CopyAsset(oldPath, newPath);
            SaveAssetsDatabase();

            return AssetDatabase.LoadAssetAtPath(newPath, typeof(T)) as T;
        }

        public static void RenameAsset(string originalPath, string fileNane)
        {
            AssetDatabase.RenameAsset(originalPath, fileNane);
            SaveAssetsDatabase();
        }

        public static void DeleteAsset<T>(string assetPath)
        {
            if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
            }
        }

        public static List<T> FindAssetsByType<T>() where T : ScriptableObject
        {
            List<T> assets = new List<T>();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        private static void SaveAssetsDatabase()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

#endif