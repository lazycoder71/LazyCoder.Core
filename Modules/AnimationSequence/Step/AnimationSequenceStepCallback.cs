using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LazyCoder.AnimationSequence
{
    public class AnimationSequenceStepCallback : AnimationSequenceStep
    {
        [SerializeField] private bool _isInserted;

        [Min(0f), ShowIf("@_isInserted")]
        [SerializeField] private float _insertTime;

        [SerializeField] private UnityEvent _callback;

        public override string DisplayName { get { return "Callback"; } }

        public override void AddToSequence(AnimationSequence animationSequence)
        {
            if (_isInserted)
                animationSequence.Sequence.InsertCallback(_insertTime, () => { _callback?.Invoke(); });
            else
                animationSequence.Sequence.AppendCallback(() => { _callback?.Invoke(); });
        }
    }
}