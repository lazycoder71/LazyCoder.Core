using TMPro;
using UnityEngine;

namespace LazyCoder.Core
{
    [RequireComponent(typeof(TMP_Text))]
    public class GuiText : MonoBase
    {
        private TMP_Text _text;

        public TMP_Text Text
        {
            get
            {
                if (_text == null)
                    _text = GetComponent<TMP_Text>();

                return _text;
            }
        }

        protected virtual void Awake()
        {
            _text = Text;
        }
    }
}
