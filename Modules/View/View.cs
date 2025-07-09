using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Diagnostics;

namespace LazyCoder.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class View : MonoBase
    {
        [Title("Config")]
        [MinValue(0f)]
        [SerializeField] private float _openDuration = 0.3f;

        [MinValue(0f)]
        [SerializeField] private float _closeDuration = 0.3f;

        [SerializeField] private bool _hideOnBlock = false;

        [FoldoutGroup("Transition", Expanded = false)]
        [SerializeField] private ViewTransitionEntity[] _transitionEntities;

        [FoldoutGroup("Event", Expanded = false)]
        [SerializeField] private UnityEvent _onOpenStart;
        [FoldoutGroup("Event")]
        [SerializeField] private UnityEvent _onOpenEnd;
        [FoldoutGroup("Event")]
        [SerializeField] private UnityEvent _onCloseStart;
        [FoldoutGroup("Event")]
        [SerializeField] private UnityEvent _onCloseEnd;
        [FoldoutGroup("Event")]
        [SerializeField] private UnityEvent _onRevealStart;
        [FoldoutGroup("Event")]
        [SerializeField] private UnityEvent _onRevealEnd;
        [FoldoutGroup("Event")]
        [SerializeField] private UnityEvent _onHideStart;
        [FoldoutGroup("Event")]
        [SerializeField] private UnityEvent _onHideEnd;

        private CancelToken _cancelToken = new CancelToken();

        private Sequence _sequence;

        private CanvasGroup _canvasGroup;

        public Sequence Sequence { get { return _sequence; } }

        public bool Interactable { get { return _canvasGroup.interactable; } set { _canvasGroup.interactable = value; } }

        public UnityEvent OnOpenStart { get { return _onOpenStart; } }
        public UnityEvent OnOpenEnd { get { return _onOpenEnd; } }
        public UnityEvent OnCloseStart { get { return _onCloseStart; } }
        public UnityEvent OnCloseEnd { get { return _onCloseEnd; } }
        public UnityEvent OnRevealStart { get { return _onRevealStart; } }
        public UnityEvent OnRevealEnd { get { return _onRevealEnd; } }
        public UnityEvent OnHideStart { get { return _onHideStart; } }
        public UnityEvent OnHideEnd { get { return _onHideEnd; } }

        #region MonoBehaviour

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            ValidateTransitionEntities();
        }

        private void OnDestroy()
        {
            // Cancel running task
            _cancelToken.Cancel();

            // Kill tweens
            _sequence?.Kill();
        }

        #endregion

        #region Function -> Private

#if UNITY_EDITOR
        [Button]
        private void Setup()
        {
            _transitionEntities = GetComponentsInChildren<ViewTransitionEntity>(true);

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.Fill();

            UnityEditor.EditorUtility.SetDirty(gameObject);
        }
#endif

        private void ConstructSequence()
        {
            if (_sequence != null)
                return;

            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            ViewExtra[] extras = GetComponents<ViewExtra>();

            if (extras.IsNullOrEmpty() && _transitionEntities.IsNullOrEmpty())
            {
                _sequence.AppendInterval(1.0f);
            }
            else
            {
                if (extras != null)
                {
                    for (int i = 0; i < extras.Length; i++)
                        extras[i].Apply(this);
                }

                for (int i = 0; i < _transitionEntities.Length; i++)
                    _transitionEntities[i].Apply(this);
            }

            _sequence.SetUpdate(true);
            _sequence.SetAutoKill(false);
        }

        private async UniTask ProcessOpen(bool isReveal)
        {
            // Handle cancel token
            _cancelToken.Cancel();

            ConstructSequence();

            // Open start callback
            if (isReveal)
                _onRevealStart?.Invoke();
            else
                _onOpenStart?.Invoke();

            // Active object when open (in case it hidden before)
            GameObjectCached.SetActive(true);

            _canvasGroup.interactable = false;

            if (_openDuration > 0.0f)
            {
                _sequence.timeScale = _openDuration > 0.0f ? 1.0f / _openDuration : 1.0f;

                _sequence.Complete();
                _sequence.Restart();
                _sequence.Play();

                await UniTask.WaitForSeconds(_openDuration, true, cancellationToken: _cancelToken.Token);
            }
            else
            {
                _sequence.Complete();
            }

            // Open end callback
            if (isReveal)
                _onRevealEnd?.Invoke();
            else
                _onOpenEnd?.Invoke();

            _canvasGroup.interactable = true;
        }

        private async UniTask ProcessClose(bool isHiding)
        {
            // Handle cancel token
            _cancelToken.Cancel();

            ConstructSequence();

            // Close start callback
            if (isHiding)
                _onHideStart?.Invoke();
            else
                _onCloseStart?.Invoke();

            _canvasGroup.interactable = false;

            if (_closeDuration > 0.0f)
            {
                _sequence.timeScale = _closeDuration > 0.0f ? 1.0f / _closeDuration : 1.0f;

                _sequence.Complete();
                _sequence.PlayBackwards();

                await UniTask.WaitForSeconds(_closeDuration, true, cancellationToken: _cancelToken.Token);
            }
            else
            {
                _sequence.Rewind();
            }

            if (isHiding)
            {
                _onHideEnd?.Invoke();

                GameObjectCached.SetActive(false);
            }
            else
            {
                _onCloseEnd?.Invoke();
            }
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        private void ValidateTransitionEntities()
        {
            for (int i = 0; i < _transitionEntities.Length; i++)
            {
                if (_transitionEntities[i] == null)
                {
                    LDebug.LogError<View>($"View {gameObject.name}, {typeof(ViewTransitionEntity)} at index {i} is null!");
                    return;
                }
            }

            ViewTransitionEntity[] entities = GetComponentsInChildren<ViewTransitionEntity>(true);

            if (entities.Length != _transitionEntities.Length)
                LDebug.LogWarning<View>($"View {gameObject.name}, not all {typeof(ViewTransitionEntity)} is referenced");
        }

        #endregion

        #region Function -> Public

        public void Open()
        {
            ProcessOpen(false).Forget();
        }

        public void Close()
        {
            ProcessClose(false).Forget();
        }

        public void Reveal()
        {
            _canvasGroup.interactable = true;

            if (_hideOnBlock)
                ProcessOpen(true).Forget();
        }

        public void Block()
        {
            _canvasGroup.interactable = false;

            if (_hideOnBlock)
                ProcessClose(true).Forget();
        }

        #endregion
    }
}