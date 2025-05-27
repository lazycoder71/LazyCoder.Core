using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LFramework.View
{
    public class ViewContainer : MonoSingleton<ViewContainer>
    {
        private List<View> _views = new List<View>();

        private bool _isTransiting = false;

        protected override bool PersistAcrossScenes { get { return false; } }

        #region Function -> Private

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

        public async UniTask<View> PushAsync(AssetReference viewAsset, CancellationToken cancelToken = new CancellationToken())
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