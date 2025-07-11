using TMPro;
using UnityEngine;

namespace LazyCoder.Core
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class GuiText : MonoBase
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
