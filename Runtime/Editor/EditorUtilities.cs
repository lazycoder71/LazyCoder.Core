using System.IO;
using UnityEngine;
using UnityEditor;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;

namespace LazyCoder.Core.Editor
{
    public static class EditorUtilities
    {
        #region Data

        [MenuItem("LazyCoder/Data/Clear PlayerPrefs", false)]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();

            LDebug.Log<EditorUtility>("PlayerPrefs cleared!");
        }

        [MenuItem("LazyCoder/Data/Clear Game Data", false)]
        private static void ClearGameData()
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            LDebug.Log<EditorUtility>("Game data cleared!");
        }

        [MenuItem("LazyCoder/Data/Clear Caching", false)]
        private static void ClearCache()
        {
            Caching.ClearCache();

            LDebug.Log<EditorUtility>("Caching cleared!");
        }

        [MenuItem("LazyCoder/Data/Clear All", false)]
        private static void ClearAll()
        {
            ClearPlayerPrefs();
            ClearGameData();
            ClearCache();
        }

        [MenuItem("LazyCoder/Data/Open GameData Directory", false)]
        private static void OpenGameData()
        {
            EditorFileBrowser.OpenDirectory(Application.persistentDataPath);
        }

        #endregion

        #region Game

        private const float SlowTimeScale = 0.1f;

        private static bool _slowed = false;

        [MenuItem("LazyCoder/Game/Pause or Resume _F2", false)]
        private static void Pause()
        {
            if (!Application.isPlaying)
                return;

            if (!EditorApplication.isPaused)
            {
                Debug.Break();
            }
            else
            {
                EditorApplication.isPaused = false;
            }
        }

        [MenuItem("LazyCoder/Game/Slow or Resume _F3", false)]
        private static void Slow()
        {
            if (!Application.isPlaying)
                return;

            if (_slowed)
            {
                _slowed = false;
                Time.timeScale = 1f;
            }
            else
            {
                _slowed = true;
                Time.timeScale = SlowTimeScale;
            }
        }

        [MenuItem("LazyCoder/Game/Reload scene _F5", false)]
        private static void ReloadScene()
        {
            if (!Application.isPlaying)
                return;

            // Reload scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

        [MenuItem("LazyCoder/Capture Screenshot %_print", false)]
        private static void CaptureScreenshot()
        {
            var currentTime = DateTime.Now;
            var filename = currentTime.ToString(CultureInfo.InvariantCulture).Replace('/', '-').Replace(':', '_') + ".png";
            var path = "Assets/" + filename;

            ScreenCapture.CaptureScreenshot(path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}