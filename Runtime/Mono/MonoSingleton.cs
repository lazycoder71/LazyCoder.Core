using System.Threading;
using UnityEngine;

namespace LazyCoder.Core
{
    /// <summary>
    /// Singleton pattern implementation for MonoBehaviour classes.
    /// Features:
    /// - Lazy initialization
    /// - Optional DontDestroyOnLoad support
    /// - Thread-safe instance access
    /// - Proper cleanup on application quit
    /// - Protection against accessing destroyed instances
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public abstract class MonoSingleton<T> : MonoBase where T : MonoSingleton<T>
    {
        /// <summary>
        /// Should this singleton persist across scene loads?
        /// </summary>
        protected abstract bool PersistAcrossScenes { get; }

        private static T _instance;

        private static readonly object Lock = new();

        /// <summary>
        /// Returns the singleton instance. If it doesn't exist, tries to find one in the scene.
        /// If none exists, returns null (doesn't auto-create to avoid unexpected behavior).
        /// </summary>
        public static T Instance
        {
            get
            {
                if (IsDestroyed)
                {
                    LDebug.LogError<T>(
                        $"{typeof(T)} is already destroyed. " +
                        $"Check {nameof(HasInstance)} or {nameof(IsDestroyed)} before accessing instance in the destructor.");
                    return null;
                }

                if (_instance)
                    return _instance;

                lock (Lock)
                {
                    if (!_instance)
                        _instance = FindFirstObjectByType<T>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Returns the singleton instance. If it doesn't exist, creates one automatically.
        /// </summary>
        public static T SafeInstance
        {
            get
            {
                if (!Instance)
                {
                    CreateInstance();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Returns true if Singleton Instance exists.
        /// </summary>
        public static bool HasInstance => _instance != null && !IsDestroyed;

        /// <summary>
        /// Returns true if the singleton was explicitly destroyed or destroyed during application quit.
        /// </summary>
        public static bool IsDestroyed { get; private set; }

        /// <summary>
        /// Creates a new instance of the singleton if one doesn't already exist.
        /// </summary>
        /// <returns>The singleton instance</returns>
        private static void CreateInstance()
        {
            if (HasInstance)
            {
                LDebug.LogWarning<T>($"Attempting to create {typeof(T).Name} instance, but one already exists.");
                return;
            }

            lock (Lock)
            {
                if (_instance == null && !IsDestroyed)
                {
                    var gameObject = new GameObject($"[Singleton] {typeof(T).Name}");
                    var created = gameObject.AddComponent<T>();
                    Interlocked.CompareExchange(ref _instance, created, null);
                    IsDestroyed = false;

                    LDebug.Log<T>($"Created singleton instance of {typeof(T).Name}");
                }
            }
        }

        #region MonoBehaviour

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                IsDestroyed = false;
                _instance = this as T;

                if (PersistAcrossScenes)
                {
                    DontDestroyOnLoad(GameObjectCached);
                    LDebug.Log<T>($"Singleton {typeof(T).Name} marked as DontDestroyOnLoad");
                }
            }
            else if (_instance != this)
            {
                LDebug.Log<T>($"Duplicate singleton {typeof(T).Name} detected. Destroying duplicate.");
                Destroy(GameObjectCached);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance != this)
                return;

            _instance = null;
            IsDestroyed = true;
        }

        #endregion
    }
}