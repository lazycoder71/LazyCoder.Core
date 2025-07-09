using DG.Tweening;
using UnityEngine;

namespace LazyCoder.View
{
    [System.Serializable]
    public abstract class ViewExtra : MonoBehaviour
    {
        public void Apply(View view)
        {
            Tween tween = GetTween(view, 1.0f);

            view.Sequence.Join(tween);
        }

        protected abstract Tween GetTween(View popup, float duration);
    }
}
