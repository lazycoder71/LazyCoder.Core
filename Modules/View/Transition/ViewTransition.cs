using DG.Tweening;
using System;

namespace LazyCoder.View
{
    [Serializable]
    public abstract class ViewTransition
    {
        public abstract string DisplayName { get; }

        public abstract Tween GetTween(ViewTransitionEntity entity, float duration);
    }
}
