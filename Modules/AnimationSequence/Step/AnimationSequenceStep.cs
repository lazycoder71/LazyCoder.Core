using System;

namespace LazyCoder.AnimationSequence
{
    [Serializable]
    public abstract class AnimationSequenceStep
    {
        [Serializable]
        public enum AddType
        {
            Append = 0,
            Join = 1,
            Insert = 2,
        }

        public abstract string DisplayName { get; }

        public abstract void AddToSequence(AnimationSequence animationSequence);
    }
}