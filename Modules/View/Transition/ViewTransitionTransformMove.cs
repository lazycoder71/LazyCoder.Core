using DG.Tweening;
using UnityEngine;

namespace LazyCoder.View
{
    public class ViewTransitionTransformMove : ViewTransitionTransform
    {
        public override string DisplayName { get { return "Transform Move"; } }

        public override Tween GetTween(ViewTransitionEntity entity, float duration)
        {
            Vector3 value = _keepEnd ? entity.RectTransform.anchoredPosition3D : _valueEnd;
            Vector3 valueStart = _keepStart ? entity.RectTransform.anchoredPosition3D : _valueStart;

            return entity.RectTransform.DOAnchorPos3D(value, duration)
                                       .ChangeStartValue(valueStart)
                                       .SetEase(_ease);
        }
    }
}
