using UnityEngine;

namespace LazyCoder.Core
{
    public class MonoBase : MonoBehaviour
    {
        private GameObject _gameObject;

        private Transform _transform;

        private bool _isStarted = false;

        public Transform TransformCached
        {
            get
            {
                if (_transform == null)
                    _transform = transform;

                return _transform;
            }
        }

        public GameObject GameObjectCached
        {
            get
            {
                if (_gameObject == null)
                    _gameObject = gameObject;

                return _gameObject;
            }
        }

        protected virtual void OnEnable()
        {
            // Prevents tick functions called before "Start"
            if (!_isStarted)
                return;

            RegisterTick();
        }

        protected virtual void OnDisable()
        {
            if (MonoCallback.IsDestroyed)
                return;

            UnregisterTick();
        }

        protected virtual void Start()
        {
            _isStarted = true;

            RegisterTick();
        }

        private void RegisterTick()
        {
            MonoCallback.Instance.EventUpdate += Tick;
            MonoCallback.Instance.EventLateUpdate += LateTick;
            MonoCallback.Instance.EventFixedUpdate += FixedTick;
        }

        private void UnregisterTick()
        {
            MonoCallback.Instance.EventUpdate -= Tick;
            MonoCallback.Instance.EventLateUpdate -= LateTick;
            MonoCallback.Instance.EventFixedUpdate -= FixedTick;
        }

        protected virtual void Tick()
        {
        }

        protected virtual void LateTick()
        {
        }

        protected virtual void FixedTick()
        {
        }
    }
}
