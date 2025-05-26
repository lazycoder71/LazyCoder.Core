using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

namespace LFramework.SceneLoader
{
    public class SceneLoader : MonoSingleton<SceneLoader>
    {
        private static readonly float s_loadSceneProgressMax = 0.7f;

        [Title("Event")]
        [SerializeField] private UnityEvent _eventFadeInStart;
        [SerializeField] private UnityEvent _eventFadeInEnd;
        [SerializeField] private UnityEvent _eventFadeOutStart;
        [SerializeField] private UnityEvent _eventFadeOutEnd;

        [SerializeField] private UnityEvent<float> _onLoadProgress;

        [Title("Config")]
        [SerializeField] private float _fadeInDuration;
        [SerializeField] private float _fadeOutDuration;
        [SerializeField] private float _loadMinDuration;

        private bool _isTransiting = false;

        protected override bool PersistAcrossScenes { get { return true; } }

        private async UniTaskVoid LoadAsync(AsyncOperation asyncOperation)
        {
            GameObjectCached.SetActive(true);

            _isTransiting = true;

            // Progress event at 0
            _onLoadProgress?.Invoke(0f);

            // Fade in start
            _eventFadeInStart?.Invoke();

            // Wait for fade in duration
            await UniTask.WaitForSeconds(_fadeInDuration, true);

            // Fade in end
            _eventFadeInEnd?.Invoke();

            // Wait for scene load complete or min duration passed
            await WaitForSceneLoadedOrMinDuration(asyncOperation);

            // Fade out start
            _eventFadeOutStart?.Invoke();

            // Wait for fade out duration
            await UniTask.WaitForSeconds(_fadeOutDuration, true);

            _eventFadeOutEnd?.Invoke();

            _isTransiting = false;
        }

        private async UniTask WaitForSceneLoadedOrMinDuration(AsyncOperation handle)
        {
            var progress = Progress.CreateOnlyValueChanged<float>(x => _onLoadProgress?.Invoke(x * s_loadSceneProgressMax));

            float timeStartLoading = Time.unscaledTime;

            handle.allowSceneActivation = true;

            await handle.ToUniTask(progress);

            await Resources.UnloadUnusedAssets();

            float timeRemain = _loadMinDuration - (Time.unscaledTime - timeStartLoading);

            float time = 0f;

            while (time < timeRemain)
            {
                time += Time.unscaledDeltaTime;

                await UniTask.Yield();

                _onLoadProgress?.Invoke((time / timeRemain) * (1f - s_loadSceneProgressMax) + s_loadSceneProgressMax);
            }

            _onLoadProgress?.Invoke(1f);
        }

        #region Public

        public void Load(string sceneName)
        {
            if (_isTransiting)
            {
                LDebug.Log<SceneLoader>("A scene is transiting, can't execute load scene command!");
                return;
            }

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;

            LoadAsync(asyncOperation).Forget();
        }

        public void Load(int sceneBuildIndex)
        {
            if (_isTransiting)
            {
                LDebug.Log<SceneLoader>("A scene is transiting, can't execute load scene command!");
                return;
            }

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex);
            asyncOperation.allowSceneActivation = false;

            LoadAsync(asyncOperation).Forget();
        }

        #endregion
    }

    public static class SceneLoaderHelper
    {
        private static bool _isInitialized = false;

        private static void LazyInit()
        {
            if (_isInitialized)
                return;

            SceneLoaderSos.Prefab.Create();

            _isInitialized = true;
        }

        public static void Load(int sceneBuildIndex)
        {
            LazyInit();

            SceneLoader.Instance.Load(sceneBuildIndex);
        }

        public static void Load(string sceneName)
        {
            LazyInit();

            SceneLoader.Instance.Load(sceneName);
        }

        public static void Reload()
        {
            LazyInit();

            SceneLoader.Instance.Load(SceneManager.GetActiveScene().buildIndex);
        }
    }
}