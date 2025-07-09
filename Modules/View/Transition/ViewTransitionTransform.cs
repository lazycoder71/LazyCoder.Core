using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.View
{
    public abstract class ViewTransitionTransform : ViewTransition
    {
        [SerializeField] protected Ease _ease = Ease.Linear;

        [SerializeField] protected bool _keepEnd = true;

        [HideIf("@_keepEnd")]
        [SerializeField] protected Vector3 _valueEnd;

        [SerializeField] protected bool _keepStart = false;

        [HideIf("@_keepStart")]
        [SerializeField] protected Vector3 _valueStart;
    }
}
