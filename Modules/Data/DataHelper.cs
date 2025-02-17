using OdinSerializer;
using System;
using System.IO;
using UnityEngine;

namespace LFramework.Data
{
    public static class DataHelper
    {
        private static readonly string s_deviceFolderName = "LFramework";

        private static void Log(string message)
        {
            LDebug.Log(typeof(DataHelper), message);
        }

        private static string GetDevicePath(string filePath)
        {
            return Path.Combine(Application.persistentDataPath, s_deviceFolderName, filePath);
        }

        private static string GetProjectPath(string filePath)
        {
            return Path.Combine(Application.dataPath, filePath);
        }

        private static void Save<T>(T data, string filePath) where T : class
        {
            try
            {
                byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.Binary);

                // Create folder if needed
                {
                    string directionRoot = Path.GetDirectoryName(filePath);

                    if (!Directory.Exists(directionRoot))
                        Directory.CreateDirectory(directionRoot);
                }

                File.WriteAllBytes(filePath, bytes);
            }
            catch (Exception e)
            {
                Log($"Save failed: {e}");
            }
        }

        private static T Load<T>(string filePath) where T : class
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Log($"Can't load, file {filePath} does not exist! creating new file");
                    return null;
                }

                byte[] bytes = File.ReadAllBytes(filePath);

                return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
            }
            catch (Exception e)
            {
                Log($"Load failed: {e}");

                return null;
            }
        }

        private static void Delete(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Log($"Can't delete, file {filePath} does not exist!");
                    return;
                }

                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Log($"Delete failed: {e}");
            }
        }

        public static T Load<T>(TextAsset textAsset) where T : class
        {
            try
            {
                return SerializationUtility.DeserializeValue<T>(textAsset.bytes, DataFormat.Binary);
            }
            catch (Exception e)
            {
                Log($"Load failed: {e}");

                return null;
            }
        }

        public static void SaveToDevice<T>(T data, string filePath) where T : class
        {
            Save(data, GetDevicePath(filePath));
        }

        public static void SaveToProject<T>(T data, string filePath) where T : class
        {
            Save(data, GetProjectPath(filePath));
        }

        public static T LoadFromDevice<T>(string filePath) where T : class
        {
            return Load<T>(GetDevicePath(filePath));
        }

        public static T LoadFromProject<T>(string filePath) where T : class
        {
            return Load<T>(GetProjectPath(filePath));
        }

        public static void DeleteInDevice(string filePath)
        {
            Delete(GetDevicePath(filePath));
        }

        public static void DeleteInProject(string filePath)
        {
            Delete(GetProjectPath(filePath));
        }

        public static void DeleteAllInDevice()
        {
            string path = Path.Combine(Application.persistentDataPath, s_deviceFolderName);

            var info = new DirectoryInfo(path);

            if (!info.Exists)
                return;

            var files = info.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
            }
        }
    }
}