using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace LFramework.View
{
    public class ViewContainer : MonoSingleton<ViewContainer>
    {
        protected override bool _dontDestroyOnLoad { get { return false; } }

        private List<View> _views = new List<View>();

        private bool _isTransiting = false;

        #region Function -> Private

        protected override void OnEnable()
        {
            base.OnEnable();

            SceneManager.activeSceneChanged += SceneManager_ActiveSceneChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SceneManager.activeSceneChanged -= SceneManager_ActiveSceneChanged;
        }

        private void SceneManager_ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            // Close all view when scene changed
            for (int i = _views.Count - 1; i >= 0; i--)
                _views[i].Close();
        }

        private View GetTopView()
        {
            return _views.Count <= 0 ? null : _views.Last();
        }

        private void PopTopView()
        {
            _views.Pop();
        }

        private void RevealTopView()
        {
            View topView = GetTopView();

            topView?.Reveal();
        }

        private void BlockTopView()
        {
            View topView = GetTopView();

            topView?.Block();
        }

        private bool CanPushNewView(object view)
        {
            // Can't push another view when it is transiting
            if (_isTransiting)
            {
                LDebug.Log<ViewContainer>($"Another View is transiting, can't push any new view {view}");
                return false;
            }

            return true;
        }

        private void OpenView(View view)
        {
            BlockTopView();

            // Handle view callback
            view.OnCloseStart.AddListener(PopTopView);
            view.OnCloseEnd.AddListener(() =>
            {
                Destroy(view.GameObjectCached);

                RevealTopView();
            });

            // Open new view
            view.Open();

            // Push new view into stack
            _views.Add(view);
        }

        #endregion

        #region Function -> Public

        public async UniTask<View> PushAsync(AssetReference viewAsset, CancellationToken cancelToken)
        {
            if (!CanPushNewView(viewAsset))
                return null;

            // Set transiting flag
            _isTransiting = true;

            // Wait new view to be loaded
            var handle = Addressables.LoadAssetAsync<GameObject>(viewAsset);

            await handle.WithCancellation(cancelToken);

            // Spawn view object from loaded asset
            View view = handle.Result.Create(TransformCached, false).GetComponent<View>();
            view.GameObjectCached.SetActive(false);

            // Release asset when view closed
            view.OnCloseEnd.AddListener(() => { handle.Release(); });

            OpenView(view);

            _isTransiting = false;

            return view;
        }

        public View Push(GameObject viewPrefab)
        {
            if (!CanPushNewView(viewPrefab))
                return null;

            // Spawn view object from prefab
            View view = viewPrefab.Create(TransformCached, false).GetComponent<View>();
            view.GameObjectCached.SetActive(false);

            OpenView(view);

            return view;
        }

        #endregion
    }
}