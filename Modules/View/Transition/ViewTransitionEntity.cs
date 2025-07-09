using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.View
{
    public class ViewTransitionEntity : MonoBase
    {
        [Title("Config")]
        [Range(0.1f, 1f)]
        [SerializeField] private float _duration = 1f;

        [CustomValueDrawer("GUIDrawInsertTime")]
        [SerializeField] private float _insertTime = 0f;

        [ListDrawerSettings(AddCopiesLastElement = true, ListElementLabelName = "DisplayName")]
        [SerializeReference] private ViewTransition[] _transitions = new ViewTransition[0];

        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        public void Apply(View view)
        {
            for (int i = 0; i < _transitions.Length; i++)
            {
                Tween tween = _transitions[i].GetTween(this, _duration);

                if (_insertTime > 0f)
                    view.Sequence.Insert(_insertTime, tween);
                else
                    view.Sequence.Join(tween);
            }
        }

#if UNITY_EDITOR

        private float GUIDrawInsertTime(float insertTime, GUIContent label)
        {
            return UnityEditor.EditorGUILayout.Slider(label, insertTime, 0f, 1.0f - _duration);
        }

#endif
    }
}
