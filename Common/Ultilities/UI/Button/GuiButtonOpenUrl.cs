using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder
{
    public class GuiButtonOpenUrl : GuiButton
    {
        [Title("Config")]
        [SerializeField] private string _strUrl;

        public override void Button_OnClick()
        {
            base.Button_OnClick();

            Application.OpenURL(_strUrl);
        }
    }
}