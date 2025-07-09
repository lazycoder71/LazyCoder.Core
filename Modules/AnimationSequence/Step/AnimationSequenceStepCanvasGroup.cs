using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.AnimationSequence
{
    public class AnimationSequenceStepCanvasGroup : AnimationSequenceStepAction<CanvasGroup>
    {
        [Range(0f, 1f), ShowIf("@_changeStartValue")]
        [SerializeField] private float _alphaStart = 0.0f;

        [Range(0f, 1f)]
        [SerializeField] private float _alpha = 1.0f;

        public override string DisplayName { get { return $"{((_isSelf || _owner == null) ? "CanvasGroup (This)" : _owner.name)}: DOFade"; } }

        protected override Tween GetTween(AnimationSequence animationSequence)
        {
            CanvasGroup owner = _isSelf ? animationSequence.GetComponent<CanvasGroup>() : _owner;

            float duration = _isSpeedBased ? Mathf.Abs(_alpha - owner.alpha) / _duration : _duration;
            float start = _changeStartValue ? _alphaStart : owner.alpha;
            float end = _relative ? owner.alpha + _alpha : _alpha;

            Tween tween = owner.DOFade(end, duration)
                               .ChangeStartValue(start);

            return tween;
        }

        protected override Tween GetResetTween(AnimationSequence animationSequence)
        {
            CanvasGroup owner = _isSelf ? animationSequence.GetComponent<CanvasGroup>() : _owner;

            return owner.DOFade(owner.alpha, 0.0f);
        }
    }
}