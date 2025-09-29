﻿using System;
using UnityEngine.SceneManagement;

namespace LazyCoder.Core
{
    /// <summary>
    /// The MonoCallback helper class is meant to be used when you need to have MonoBehaviour default Unity callbacks,
    /// but your model is not a MonoBehaviour and you dont want to convert it to the MonoBehaviour by design.
    ///
    /// Please note, that you will subscribe to the global MonoBehaviour singleton instance. Other parts of code may also use it.
    /// In case other callback users will throw and unhandled exception you may not received the callback you subscribed for.
    /// </summary>
    public class MonoCallback : MonoSingleton<MonoCallback>
    {
        protected override bool PersistAcrossScenes => true;

        /// <summary>
        /// Update is called every frame.
        /// Learn more: [MonoBehaviour.Update](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html)
        /// </summary>
        public event Action EventUpdate;

        /// <summary>
        /// LateUpdate is called after all Update functions have been called. This is useful to order script execution.
        /// For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.
        /// Learn more: [MonoBehaviour.LateUpdate](https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html)
        /// </summary>
        public event Action EventLateUpdate;

        /// <summary>
        /// In the editor this is called when the user stops playmode.
        /// Learn more: [MonoBehaviour.OnApplicationQuit](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html)
        /// </summary>
        public event Action EventApplicationQuit;

        /// <summary>
        /// Sent to all GameObjects when the application pauses.
        /// Learn more: [MonoBehaviour.OnApplicationPause](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html)
        /// </summary>
        public event Action<bool> EventApplicationPause;

        /// <summary>
        /// Sent to all GameObjects when the player gets or loses focus.
        /// Learn more: [MonoBehaviour.OnApplicationFocus](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationFocus.html)
        /// </summary>
        public event Action<bool> EventApplicationFocus;

        /// <summary>
        /// Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations.
        /// Learn more: [MonoBehaviour.FixedUpdate](https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html)
        /// </summary>
        public event Action EventFixedUpdate;

        /// <summary>
        /// Called when active scene changed.
        /// </summary>
        public event Action<Scene, Scene> EventActiveSceneChanged;

        private void Update() => EventUpdate?.Invoke();
        
        private void LateUpdate() => EventLateUpdate?.Invoke();
        
        private void FixedUpdate() => EventFixedUpdate?.Invoke();
        
        private void OnApplicationPause(bool pauseStatus) => EventApplicationPause?.Invoke(pauseStatus);
        
        private void OnApplicationFocus(bool hasFocus) => EventApplicationFocus?.Invoke(hasFocus);

        protected override void Awake()
        {
            base.Awake();

            SceneManager.activeSceneChanged += SceneManager_ActiveSceneChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.activeSceneChanged -= SceneManager_ActiveSceneChanged;
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            EventApplicationQuit?.Invoke();
        }

        private void SceneManager_ActiveSceneChanged(Scene scenePrevious, Scene sceneCurrent)
        {
            EventActiveSceneChanged?.Invoke(scenePrevious, sceneCurrent);
        }
    }
}
