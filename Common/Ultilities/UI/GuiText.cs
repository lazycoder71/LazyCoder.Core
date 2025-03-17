using TMPro;
using UnityEngine;

namespace LFramework
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class GuiText : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        public TextMeshProUGUI Text
        {
            get
            {
                if (_text == null)
                    _text = GetComponent<TextMeshProUGUI>();

                return _text;
            }
        }

        protected virtual void Awake()
        {
            _text = Text;
        }
    }
}
