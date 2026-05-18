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
                if (!_transform)
                    _transform = transform;

                return _transform;
            }
        }

        public GameObject GameObjectCached
        {
            get
            {
                if (!_gameObject)
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
            if (!_isStarted)
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
            MonoCallback.SafeInstance.EventUpdate += Tick;
            MonoCallback.SafeInstance.EventLateUpdate += LateTick;
            MonoCallback.SafeInstance.EventFixedUpdate += FixedTick;
        }

        private void UnregisterTick()
        {
            if (MonoCallback.IsDestroyed)
                return;
            
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
