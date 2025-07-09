using DG.Tweening;
using UnityEngine;

namespace LazyCoder
{
    public class LCollectStepInterval : LCollectStep
    {
        [SerializeField] private float _duration;

        public override void Apply(LCollectItem item)
        {
            item.Sequence.AppendInterval(_duration);
        }
    }
}
