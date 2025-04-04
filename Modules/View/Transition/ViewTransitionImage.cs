using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LFramework.View
{
    public class ViewTransitionImage : ViewTransition
    {
        [SerializeField] private Ease _ease = Ease.Linear;

        [SerializeField] private bool _keepEnd = true;

        [HideIf("@_keepEnd"), Range(0f, 1f)]
        [SerializeField] private Color _colorEnd = Color.white;

        [SerializeField] private bool _keepStart = false;

        [HideIf("@_keepStart"), Range(0f, 1f)]
        [SerializeField] private Color _colorStart = Color.white;

        public override string DisplayName { get { return "Image"; } }

        public override Tween GetTween(ViewTransitionEntity entity, float duration)
        {
            Image image = entity.GetComponent<Image>();

            if (image == null)
                return null;

            Color colorEnd = _keepEnd ? image.color : _colorEnd;
            Color colorStart = _keepStart ? image.color : _colorStart;

            return image.DOColor(colorEnd, duration)
                        .ChangeStartValue(colorStart)
                        .SetEase(_ease);
        }
    }
}
