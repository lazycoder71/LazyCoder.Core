using DG.Tweening;
using UnityEngine;

namespace LazyCoder.AnimationSequence
{
    public class AnimationSequenceStepInterval : AnimationSequenceStep
    {
        [SerializeField] private float _duration;

        public override string DisplayName { get { return "Interval"; } }

        public override void AddToSequence(AnimationSequence animationSequence)
        {
            animationSequence.Sequence.AppendInterval(_duration);
        }
    }
}