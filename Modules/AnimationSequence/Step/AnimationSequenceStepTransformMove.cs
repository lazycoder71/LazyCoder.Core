using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LFramework.AnimationSequence
{
    public class AnimationSequenceStepTransformMove : AnimationSequenceStepTransform
    {
        [VerticalGroup("Value")]
        [SerializeField] private bool _snapping = false;

        public override string DisplayName { get { return $"{(_isSelf || _owner == null ? "Transform (This)" : _owner.name)}: DOLocalMove"; } }

        protected override Tween GetTween(AnimationSequence animationSequence)
        {
            Transform owner = _isSelf ? animationSequence.Transform : _owner;

            float duration = _isSpeedBased ? Vector3.Distance(_value, owner.localPosition) / _duration : _duration;

            Tweener tween = owner.DOLocalMove(_relative ? owner.localPosition + _value : _value, duration, _snapping);

            if (_changeStartValue)
                tween.ChangeStartValue(_relative ? owner.localPosition + _valueStart : _valueStart);
            else
                tween.ChangeStartValue(owner.localPosition);

            owner.localPosition = _relative ? owner.localPosition + _value : _value;

            return tween;
        }

        protected override Tween GetResetTween(AnimationSequence animationSequence)
        {
            Transform owner = _isSelf ? animationSequence.Transform : _owner;

            return owner.DOLocalMove(owner.localPosition, 0.0f);
        }
    }
}