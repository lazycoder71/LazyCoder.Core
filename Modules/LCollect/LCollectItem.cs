using DG.Tweening;
using LFramework.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LFramework
{
    public class LCollectItem : MonoBase
    {
        [Title("Event")]
        [SerializeField] private UnityEvent _eventConstructed;
        [SerializeField] private UnityEvent _eventCollected;

        private Sequence _sequence;

        private LCollectDestination _destination;

        private RectTransform _rectTransform;

        public Sequence Sequence { get { return _sequence; } }

        public LCollectDestination Destination { get { return _destination; } }

        public RectTransform RectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        public void Construct(LCollectConfig config, LCollectDestination destination)
        {
            _destination = destination;

            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            for (int i = 0; i < config.steps.Length; i++)
            {
                config.steps[i].Apply(this);
            }

            _sequence.OnComplete(() =>
            {
                _destination.Collect();

                PoolPrefabShared.Release(GameObjectCached);

                _eventCollected?.Invoke();
            });

            _eventConstructed?.Invoke();
        }
    }
}
