using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.View
{
    public class ViewTransitionCanvasGroup : ViewTransition
    {
        [SerializeField] private Ease _ease = Ease.Linear;

        [SerializeField] private bool _keepEnd = true;

        [HideIf("@_keepEnd"), Range(0f, 1f)]
        [SerializeField] private float _alphaEnd;

        [SerializeField] private bool _keepStart = false;

        [HideIf("@_keepStart"), Range(0f, 1f)]
        [SerializeField] private float _alphaStart;

        public override string DisplayName { get { return "Canvas Group"; } }

        public override Tween GetTween(ViewTransitionEntity entity, float duration)
        {
            CanvasGroup canvasGroup = entity.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                return null;

            float alphaEnd = _keepEnd ? canvasGroup.alpha : _alphaEnd;
            float alphaStart = _keepStart ? canvasGroup.alpha : _alphaStart;

            return canvasGroup.DOFade(alphaEnd, duration)
                              .ChangeStartValue(alphaStart)
                              .SetEase(_ease);
        }
    }
}
