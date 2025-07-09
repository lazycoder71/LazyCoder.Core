using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder
{
    public class SpriteRendererAnimated : MonoBase
    {
        [Header("Config")]
        [SerializeField] private Sprite[] _frames;

        [Min(1)]
        [SerializeField] private int _fps = 30;

        [SerializeField] private int _loopCount = 0;

        [ShowIf("@_loopCount < 0")]
        [SerializeField] private LoopType _loopType;

        private SpriteRenderer _spriteRenderer;

        private Sequence _sequence;

        public Sequence Sequence
        {
            get
            {
                if (_sequence == null)
                    InitSequence();

                return _sequence;
            }
        }

        #region MonoBehaviour

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            InitSequence();
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _sequence?.Restart();
            _sequence?.Play();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _sequence?.Pause();
        }

        #endregion

        private void InitSequence()
        {
            if (_sequence != null)
                return;

            float delayBetween = 1.0f / _fps;

            _sequence = DOTween.Sequence();

            for (int i = 0; i < _frames.Length; i++)
            {
                int frameIndex = i;

                _sequence.AppendCallback(() => { _spriteRenderer.sprite = _frames[frameIndex]; });
                _sequence.AppendInterval(delayBetween);
            }

            _sequence.SetLoops(_loopCount, _loopType);
            _sequence.SetAutoKill(false);
        }
    }
}