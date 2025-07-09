using DG.Tweening;
using UnityEngine;

namespace LazyCoder.AnimationSequence
{
    public class AnimationSequenceStepRectTransformAnchorPos3D : AnimationSequenceStepRectTransform
    {
        public override string DisplayName { get { return $"{(_isSelf || _owner == null ? "RectTransform (This)" : _owner.name)}: DOAnchorPos3D"; } }

        protected override Tween GetTween(AnimationSequence animationSequence)
        {
            RectTransform owner = _isSelf ? animationSequence.RectTransform : _owner;

            float duration = _isSpeedBased ? Vector2.Distance(_value, owner.anchoredPosition3D) / _duration : _duration;
            Vector3 start = _changeStartValue ? _valueStart : owner.anchoredPosition3D;
            Vector3 end = _relative ? owner.anchoredPosition3D + _value : _value;

            Tween tween = owner.DOAnchorPos3D(end, duration, _snapping)
                               .ChangeStartValue(start);

            return tween;
        }

        protected override Tween GetResetTween(AnimationSequence animationSequence)
        {
            RectTransform owner = _isSelf ? animationSequence.RectTransform : _owner;

            return owner.DOAnchorPos3D(owner.anchoredPosition3D, 0.0f);
        }
    }
}