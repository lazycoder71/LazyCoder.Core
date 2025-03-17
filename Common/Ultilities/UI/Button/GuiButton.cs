using UnityEngine.UI;

namespace LFramework
{
    public class GuiButton : MonoBase
    {
        private Button _button;

        public Button Button
        {
            get
            {
                if (_button == null)
                    _button = GetComponentInChildren<Button>();

                return _button;
            }
        }

        protected virtual void Awake()
        {
            Button.onClick.AddListener(Button_OnClick);
        }

        public virtual void Button_OnClick()
        {
        }
    }
}