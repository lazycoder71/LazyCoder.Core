using UnityEngine;

namespace LFramework
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
        // Use a more descriptive property name
        protected abstract bool PersistAcrossScenes { get; }

        // Use a readonly object for thread safety
        private static readonly object s_lock = new object();
        private static T s_instance;
        private static bool s_applicationIsQuitting = false;
        private static bool s_isDestroyed = false;

        /// <summary>
        /// Returns the singleton instance. If it doesn't exist, tries to find one in the scene.
        /// If none exists, returns null (doesn't auto-create to avoid unexpected behavior).
        /// </summary>
        public static T Instance
        {
            get
            {
                if (s_applicationIsQuitting || s_isDestroyed)
                {
                    LDebug.LogError<T>(
                        $"{typeof(T)} is already destroyed. " +
                        $"Please check {nameof(HasInstance)} or {nameof(IsDestroyed)} before accessing instance in the destructor.");
                    return null;
                }

                // Thread-safe check
                if (s_instance == null)
                {
                    lock (s_lock)
                    {
                        if (s_instance == null)
                        {
                            s_instance = FindFirstObjectByType<T>(); // Include inactive objects
                        }
                    }
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Returns the singleton instance. If it doesn't exist, creates one automatically.
        /// </summary>
        public static T SafeInstance
        {
            get
            {
                if (s_applicationIsQuitting)
                {
                    LDebug.LogWarning<T>($"Attempting to access {typeof(T)} after application quit.");
                    return null;
                }

                if (Instance == null)
                {
                    CreateInstance();
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Returns `true` if Singleton Instance exists.
        /// </summary>
        public static bool HasInstance => s_instance != null && !s_isDestroyed;

        /// <summary>
        /// Returns `true` if the singleton was explicitly destroyed or destroyed during application quit.
        /// </summary>
        public static bool IsDestroyed => s_isDestroyed;

        /// <summary>
        /// Creates a new instance of the singleton if one doesn't already exist.
        /// </summary>
        /// <returns>The singleton instance</returns>
        public static T CreateInstance()
        {
            if (HasInstance)
            {
                LDebug.LogWarning<T>($"Attempting to create {typeof(T).Name} instance, but one already exists.");
                return s_instance;
            }

            lock (s_lock)
            {
                if (s_instance == null && !s_applicationIsQuitting)
                {
                    // Create a new GameObject with the type name
                    var gameObject = new GameObject($"[Singleton] {typeof(T).Name}");
                    s_instance = gameObject.AddComponent<T>();
                    s_isDestroyed = false;

                    LDebug.Log<T>($"Created singleton instance of {typeof(T).Name}");
                }
            }

            return s_instance;
        }

        /// <summary>
        /// Explicitly destroys the singleton instance if it exists.
        /// </summary>
        public static void DestroyInstance()
        {
            if (s_instance != null && !s_isDestroyed)
            {
                if (Application.isPlaying)
                {
                    Destroy(s_instance.gameObject);
                }
                else
                {
                    DestroyImmediate(s_instance.gameObject);
                }

                s_instance = null;
                s_isDestroyed = true;
            }
        }

        #region MonoBehaviour

        protected virtual void Awake()
        {
            if (s_instance == null)
            {
                s_isDestroyed = false;
                s_instance = this as T;

                if (PersistAcrossScenes)
                {
                    DontDestroyOnLoad(gameObject);
                    LDebug.Log<T>($"Singleton {typeof(T).Name} marked as DontDestroyOnLoad");
                }

                OnSingletonAwake();
            }
            else if (s_instance != this)
            {
                LDebug.Log<T>($"Duplicate singleton {typeof(T).Name} detected. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Called when the singleton instance is first initialized.
        /// Override this instead of Awake for singleton-specific initialization.
        /// </summary>
        protected virtual void OnSingletonAwake() { }

        protected virtual void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
                s_isDestroyed = true;
                OnSingletonDestroyed();
            }
        }

        /// <summary>
        /// Called when the singleton instance is being destroyed.
        /// Override this instead of OnDestroy for singleton-specific cleanup.
        /// </summary>
        protected virtual void OnSingletonDestroyed() { }

        protected virtual void OnApplicationQuit()
        {
            s_applicationIsQuitting = true;

            if (s_instance == this)
            {
                s_instance = null;
                s_isDestroyed = true;
            }
        }

        #endregion
    }
}