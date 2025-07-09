using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LazyCoder.View
{
    public static class ViewHelper
    {
        public static UniTask<View> PushAsync(AssetReference viewAsset, CancellationToken cancelToken)
        {
            return ViewContainer.Instance.PushAsync(viewAsset, cancelToken);
        }

        public static View Push(GameObject viewPrefab)
        {
            return ViewContainer.Instance.Push(viewPrefab);
        }

        public static Transform GetContainterTransform()
        {
            return ViewContainer.Instance.TransformCached;
        }
    }
}