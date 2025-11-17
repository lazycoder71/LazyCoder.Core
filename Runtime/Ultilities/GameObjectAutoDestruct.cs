using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Threading;
using UnityEngine;

namespace LazyCoder.Core
{
    public class GameObjectAutoDestruct : MonoBase
    {
        [Title("Config")]
        [SerializeField] private float _delay = 0f;
        [SerializeField] private bool _ignoreTimeScale = false;
        [SerializeField] private bool _deactivateOnly = false;

        private readonly CancelToken _cancelToken = new CancelToken();

        public event Action<GameObject> EventDestruct;

        #region MonoBehaviour

        protected override void OnEnable()
        {
            base.OnEnable();

            _cancelToken.Cancel();

            DestructAsync(_cancelToken.Token).Forget();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _cancelToken.Cancel();
        }

        #endregion

        private async UniTaskVoid DestructAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), _ignoreTimeScale, cancellationToken: cancellationToken);

            if (_deactivateOnly)
                GameObjectCached.SetActive(false);
            else
                Destroy(GameObjectCached);

            EventDestruct?.Invoke(GameObjectCached);
        }
    }
}