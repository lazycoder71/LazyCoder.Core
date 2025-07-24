using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using LitMotion;
using LitMotion.Extensions;

namespace LazyCoder.Core
{
    public class GuiGraphicScale : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Title("Reference")]
        [SerializeField] private Transform _target;

        [Title("Config")]
        [SerializeField] private Vector3 _scaleStart = new Vector3(1.0f, 1.0f, 1.0f);
        [SerializeField] private Vector3 _scaleEnd = new Vector3(0.9f, 0.9f, 0.9f);

        [Min(0.1f)]
        [SerializeField] private float _scaleDuration = 0.1f;

        [SerializeField] private Ease _scaleEase = Ease.Linear;

        private bool _isDown = false;

        private bool _isScaleFoward = false;

        private MotionHandle _motion;

        #region Monobehaviour

        private void Awake()
        {
            if (_target == null)
                _target = transform;
        }

        private void OnDisable()
        {
            _motion.TryCancel();
        }

        #endregion

        #region Function -> Private

        private void InitMotion(bool isFoward)
        {
            _motion.TryCancel();
            _motion = LMotion.Create(isFoward ? 0f : 1f, isFoward ? 1f : 0f, _scaleDuration)
                             .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                             .WithEase(_scaleEase)
                             .Bind(ScaleMotion);
        }

        private void ScaleMotion(float progress)
        {
            _target.localScale = Vector3.Lerp(_scaleStart, _scaleEnd, progress);
        }

        private void ScaleForward()
        {
            if (_motion.IsActive())
            {
                double previousTime = _isScaleFoward ? _motion.Time : _motion.Duration - _motion.Time;

                InitMotion(true);

                _motion.Time = previousTime;
            }
            else
            {
                InitMotion(true);
            }

            _isScaleFoward = true;
        }

        private void ScaleBackward()
        {
            if (_motion.IsActive())
            {
                double previousTime = _isScaleFoward ? _motion.Duration - _motion.Time : _motion.Time;

                InitMotion(false);

                _motion.Time = previousTime;
            }
            else
            {
                InitMotion(false);
            }

            _isScaleFoward = false;
        }

        #endregion

        #region IPointer interfaces implement

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _isDown = true;

            ScaleForward();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (_isDown)
                ScaleBackward();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (_isDown)
                ScaleForward();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _isDown = false;

            ScaleBackward();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
        }

        #endregion
    }
}