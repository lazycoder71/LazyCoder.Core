using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.AnimationSequence
{
    public abstract class AnimationSequenceStepRectTransform : AnimationSequenceStepAction<RectTransform>
    {
        [ShowIf("@_changeStartValue")]
        [SerializeField] protected Vector3 _valueStart;

        [SerializeField] protected Vector3 _value;

        [VerticalGroup("Value")]
        [SerializeField] protected bool _snapping = false;
    }
}