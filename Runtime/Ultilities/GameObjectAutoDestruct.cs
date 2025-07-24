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
        [SerializeField] private bool _deactiveOnly = false;

        private CancelToken _cancelToken = new CancelToken();

        public event Action<GameObject> EventDestruct;

        #region MonoBehaviour

        protected override void OnEnable()
        {
            base.OnEnable();

            _cancelToken.Cancel();

            DestructAsyn(_cancelToken.Token).Forget();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _cancelToken.Cancel();
        }

        #endregion

        private async UniTaskVoid DestructAsyn(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), _ignoreTimeScale, cancellationToken: cancellationToken);

            if (_deactiveOnly)
                GameObjectCached.SetActive(false);
            else
                Destroy(GameObjectCached);

            EventDestruct?.Invoke(GameObjectCached);
        }
    }
}