using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.AnimationSequence
{
    public abstract class AnimationSequenceStepAction<T> : AnimationSequenceStep where T : class
    {
        [HorizontalGroup("AddType")]
        [SerializeField] private AddType _addType = AddType.Append;

        [HorizontalGroup("AddType"), LabelWidth(75), SuffixLabel("Second(s)", true)]
        [ShowIf("@_addType == AnimationSequenceStep.AddType.Insert"), MinValue(0)]
        [SerializeField] private float _insertTime = 0.0f;

        [HideInInspector]
        [SerializeField] protected bool _isSelf = true;

        [HorizontalGroup("Owner"), ShowIf("@_isSelf == false"), LabelWidth(75)]
        [GUIColor("@_owner == null ? new Color(1.0f, 0.2f, 0.2f) : Color.white")]
        [SerializeField] protected T _owner;

        [HideInInspector]
        [SerializeField] protected bool _isSpeedBased = false;

        [Min(0.01f), SuffixLabel("@_isSpeedBased?\"Unit/Second\":\"Second(s)\"", Overlay = true)]
        [InlineButton("@_isSpeedBased = true", Label = "Duration", ShowIf = ("@_isSpeedBased == false"))]
        [InlineButton("@_isSpeedBased = false", Label = "Speed Based", ShowIf = ("@_isSpeedBased == true"))]
        [SerializeField] protected float _duration = 1.0f;

        [SerializeField] protected Ease _ease = Ease.Linear;

        [HorizontalGroup("Update")]
        [InlineButton("@_isIndependentUpdate = true", Label = "Timescale Based", ShowIf = ("@_isIndependentUpdate == false"))]
        [InlineButton("@_isIndependentUpdate = false", Label = "Independent Update", ShowIf = ("@_isIndependentUpdate == true"))]
        [SerializeField] protected UpdateType _updateType = UpdateType.Normal;

        [HideInInspector]
        [SerializeField] protected bool _isIndependentUpdate = false;

        [MinValue(0), HorizontalGroup("Loop")]
        [SerializeField] private int _loopTime = 0;

        [ShowIf("@_loopTime != 0"), HorizontalGroup("Loop"), LabelWidth(75)]
        [SerializeField] private LoopType _loopType = LoopType.Restart;

        [VerticalGroup("Value")]
        [SerializeField] protected bool _relative = true;

        [VerticalGroup("Value")]
        [SerializeField] protected bool _changeStartValue = false;
        
        public override void AddToSequence(AnimationSequence animationSequence)
        {
            if (!Application.isPlaying)
            {
                switch (_addType)
                {
                    case AddType.Append:
                        animationSequence.Sequence.Append(GetResetTween(animationSequence));
                        break;
                    case AddType.Join:
                        animationSequence.Sequence.Join(GetResetTween(animationSequence));
                        break;
                    case AddType.Insert:
                        animationSequence.Sequence.Insert(_insertTime, GetResetTween(animationSequence));
                        break;
                }
            }

            Tween tween = GetTween(animationSequence);

            tween.SetEase(_ease);
            tween.SetUpdate(_updateType, _isIndependentUpdate);
            tween.SetLoops(_loopTime, _loopType);

            switch (_addType)
            {
                case AddType.Append:
                    animationSequence.Sequence.Append(tween);
                    break;
                case AddType.Join:
                    animationSequence.Sequence.Join(tween);
                    break;
                case AddType.Insert:
                    animationSequence.Sequence.Insert(_insertTime, tween);
                    break;
            }
        }

        protected abstract Tween GetTween(AnimationSequence animationSequence);

        protected abstract Tween GetResetTween(AnimationSequence animationSequence);

        [HorizontalGroup("Owner")]
        [ShowIf("@_isSelf == true")]
        [Button("SELF", Stretch = false, ButtonAlignment = 0), GUIColor(0, 1, 0), PropertyOrder(-1)]
        private void ToggleSelf1()
        {
            _isSelf = !_isSelf;
        }

        [HorizontalGroup("Owner", Width = 65)]
        [HideIf("@_isSelf == true")]
        [Button("OTHER", ButtonAlignment = 0), GUIColor(1, 1, 0), PropertyOrder(-1)]
        private void ToggleSelf2()
        {
            _isSelf = !_isSelf;
        }
    }
}