using DG.Tweening;
using UnityEngine;

namespace LFramework
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
