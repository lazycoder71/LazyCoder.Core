using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace LFramework
{
    public class GuiGraphicScale : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Title("Reference")]
        [SerializeField] private Transform _target;

        [Title("Config")]
        [SerializeField] private Vector2 _scaleValue = new Vector2(1f, 0.9f);

        [Min(0.1f)]
        [SerializeField] private float _scaleDuration = 0.1f;

        [SerializeField] private Ease _scaleEase = Ease.Linear;

        private bool _isDown = false;

        private Tween _tween;

        #region Monobehaviour

        private void Awake()
        {
            if (_target == null)
                _target = transform;
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }

        private void OnDisable()
        {
            _tween?.Restart();
            _tween?.Kill();
            _tween = null;
        }

        #endregion

        #region Function -> Private

        private void InitTween()
        {
            if (_tween != null)
                return;

            _tween = _target.DOScale(_scaleValue.y, _scaleDuration)
                            .ChangeStartValue(Vector3.one * _scaleValue.x)
                            .SetEase(_scaleEase)
                            .SetAutoKill(false)
                            .SetUpdate(true);

            _tween.Restart();
            _tween.Pause();
        }

        private void ScaleUp()
        {
            InitTween();

            _tween.PlayForward();
        }

        private void ScaleDown()
        {
            InitTween();

            _tween.PlayBackwards();
        }

        #endregion

        #region IPointer interfaces implement

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _isDown = true;

            ScaleUp();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (_isDown)
                ScaleDown();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (_isDown)
                ScaleUp();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _isDown = false;

            ScaleDown();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
        }

        #endregion
    }
}