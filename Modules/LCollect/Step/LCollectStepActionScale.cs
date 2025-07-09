using DG.Tweening;
using UnityEngine;

namespace LazyCoder
{
    public class LCollectStepActionScale : LCollectStepAction
    {
        [SerializeField] Vector3 _value = Vector3.one;

        protected override Tween GetTween(LCollectItem item)
        {
            return item.transform.DOScale(_value, _duration)
                                       .SetEase(_ease);
        }
    }
}
