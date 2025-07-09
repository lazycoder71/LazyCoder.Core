using DG.Tweening;
using UnityEngine;

namespace LazyCoder.View
{
    public class ViewTransitionTransformRotate : ViewTransitionTransform
    {
        [SerializeField] private RotateMode _rotateMode = RotateMode.FastBeyond360;

        public override string DisplayName { get { return "Transform Rotate"; } }

        public override Tween GetTween(ViewTransitionEntity entity, float duration)
        {
            Vector3 value = _keepEnd ? entity.TransformCached.localEulerAngles : _valueEnd;
            Vector3 valueStart = _keepStart ? entity.TransformCached.localEulerAngles : _valueStart;

            return entity.TransformCached.DOLocalRotate(value, duration, _rotateMode)
                                         .ChangeStartValue(valueStart)
                                         .SetEase(_ease);
        }
    }
}
