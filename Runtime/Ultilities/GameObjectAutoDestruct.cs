using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace LazyCoder.Core
{
    public class GameObjectAutoDestruct : MonoBase
    {
        [Title("Config")]
        [SerializeField] private float _delay = 0f;
        [SerializeField] private bool _deactiveOnly = false;

        private Tween _tween;

        public event Action<GameObject> EventDestruct;

        #region MonoBehaviour

        protected override void OnEnable()
        {
            base.OnEnable();

            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(_delay, Destruct, false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventDestruct?.Invoke(GameObjectCached);
			
            _tween?.Kill();
        }

        #endregion

        private void Destruct()
        {
            if (_deactiveOnly)
                GameObjectCached.SetActive(false);
            else
                Destroy(GameObjectCached);
        }
    }
}