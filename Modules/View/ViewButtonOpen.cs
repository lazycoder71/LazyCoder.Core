using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LFramework.View
{
    public class ViewButtonOpen : GuiButton
    {
        [HideInInspector]
        [SerializeField] protected bool _useAddressable;

        [ShowIf("@_useAddressable")]
        [SerializeField] protected AssetReferenceGameObject _asset;

        [ShowIf("@!_useAddressable")]
        [SerializeField] protected GameObject _prefab;

        private CancelToken _cancelToken = new CancelToken();

        public event Action<View> EventViewOpened;

        private void OnDestroy()
        {
            _cancelToken.Cancel();
        }

        public override async void Button_OnClick()
        {
            base.Button_OnClick();

            View view = null;


            if (_useAddressable)
            {
                _cancelToken.Cancel();

                view = await ViewHelper.PushAsync(_asset, _cancelToken.Token);
            }
            else
            {
                view = ViewHelper.Push(_prefab);
            }

            EventViewOpened?.Invoke(view);

            OnViewOpened(view);
        }

        protected virtual void OnViewOpened(View view)
        {
        }

        [Title("Config")]
        [PropertyOrder(-10)]
        [Button(Name = "@_useAddressable?\"Use Addressables\":\"Use Prefab\"", Stretch = false, ButtonAlignment = 0, Style = ButtonStyle.CompactBox, DirtyOnClick = true)]
        private void ToggleAddressable()
        {
            _useAddressable = !_useAddressable;

            if (_useAddressable)
                _prefab = null;
            else
                _asset = null;
        }
    }
}