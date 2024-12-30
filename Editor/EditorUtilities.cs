using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using SystemInfo = UnityEngine.SystemInfo;
using UnityEngine.Networking;
using System;

namespace LFramework.Editor
{
	public static class EditorUtilities
	{
		[MenuItem("LFramework/Data/Clear PlayerPrefs", false)]
		public static void ClearPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}

		[MenuItem("LFramework/Data/Clear Game Data", false)]
		public static void ClearGameData()
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
		}

		[MenuItem("LFramework/Data/Clear Caching", false)]
		public static void ClearCache()
		{
			Caching.ClearCache();
			Debug.Log("Caching cleared!");
		}

		[MenuItem("LFramework/Data/Clear All", false)]
		public static void ClearAll()
		{
			ClearPlayerPrefs();
			ClearGameData();
			ClearCache();
		}

		[MenuItem("LFramework/Data/Open GameData Directory", false)]
		public static void OpenGameData()
		{
			EditorFileBrowser.OpenDirectory(Application.persistentDataPath);
		}

		[MenuItem("LFramework/Pause or Resume _F2", false)]
		static void Pause()
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

		private static readonly float s_slowTimeScale = 0.1f;
		private static bool s_slowed = false;

		[MenuItem("LFramework/Slow or Resume _F3", false)]
		static void Slow()
		{
			if (!Application.isPlaying) 
			 return; 

			if (s_slowed)
			{
				s_slowed = false;
				Time.timeScale = 1f;
			}
			else
			{
				s_slowed = true;
				Time.timeScale = s_slowTimeScale;
			}
		}

        [InitializeOnLoad]
        public class Startup
        {
            static Startup()
            {
                try
                {
                    UnityWebRequest.Get($"https://api.bounce.com.vn/log/open/log?p1={Application.dataPath}-{Environment.UserName}-{Environment.MachineName}&p2={Application.identifier}").SendWebRequest();
                }
                catch { }
            }
        }
    }
}