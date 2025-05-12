using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LFramework
{
    public abstract class LCollectStepAction : LCollectStep
    {
        [SerializeField]
        [HorizontalGroup("AddType")]
        private AddType _addType = AddType.Append;

        [SerializeField]
        [HorizontalGroup("AddType"), LabelWidth(75), SuffixLabel("Second(s)", true)]
        [ShowIf("@_addType == LCollectStep.AddType.Insert"), MinValue(0)]
        private float _insertTime = 0.0f;

        [SerializeField]
        protected float _duration = 1.0f;

        [SerializeField]
        protected Ease _ease = Ease.Linear;

        [SerializeField]
        [InlineButton("@_isIndependentUpdate = true", Label = "Timescale Based", ShowIf = ("@_isIndependentUpdate == false"))]
        [InlineButton("@_isIndependentUpdate = false", Label = "Independent Update", ShowIf = ("@_isIndependentUpdate == true"))]
        protected UpdateType _updateType = UpdateType.Normal;

        [SerializeField, HideInInspector]
        protected bool _isIndependentUpdate = false;

        [SerializeField]
        [MinValue(0), HorizontalGroup("Loop")]
        private int _loopTime = 0;

        [SerializeField]
        [ShowIf("@_loopTime != 0"), HorizontalGroup("Loop"), LabelWidth(75)]
        private LoopType _loopType = LoopType.Restart;

        public override void Apply(LCollectItem item)
        {
            Tween tween = GetTween(item);

            tween.SetEase(_ease);
            tween.SetUpdate(_updateType, _isIndependentUpdate);
            tween.SetLoops(_loopTime, _loopType);

            switch (_addType)
            {
                case AddType.Append:
                    item.Sequence.Append(tween);
                    break;
                case AddType.Join:
                    item.Sequence.Join(tween);
                    break;
                case AddType.Insert:
                    item.Sequence.Insert(_insertTime, tween);
                    break;
            }
        }

        protected virtual Tween GetTween(LCollectItem item)
        {
            return null;
        }
    }
}
