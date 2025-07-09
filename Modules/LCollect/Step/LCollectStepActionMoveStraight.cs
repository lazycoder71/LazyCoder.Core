using DG.Tweening;
using UnityEngine;

namespace LazyCoder
{
    public class LCollectStepActionMoveStraight : LCollectStepActionMove
    {
        protected override Tween GetTween(LCollectItem item)
        {
            switch (_journey)
            {
                case Journey.Spawn:
                    Vector3 endPos = item.transform.localPosition;
                    Vector3 startPos = _startAtCenter ? Vector3.zero : endPos + _startOffset * item.RectTransform.GetUnitPerPixel();

                    return item.transform.DOLocalMove(endPos, _duration)
                                               .ChangeStartValue(startPos)
                                               .SetEase(_ease);
                case Journey.Return:
                    return item.transform.DOMove(item.Destination.Position, _duration)
                                               .SetEase(_ease);
                default:
                    return null;
            }
        }
    }
}
