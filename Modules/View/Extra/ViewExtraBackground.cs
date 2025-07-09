using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LazyCoder.View
{
    [System.Serializable]
    public class ViewExtraBackground : ViewExtra
    {
        [Title("Config")]
        [SerializeField] private bool _closeOnClick = true;
        [SerializeField] private Sprite _sprite;
        [ShowIf("@_sprite")]
        [SerializeField] private Image.Type _imageType = Image.Type.Simple;
        [ShowIf("@_sprite")]
        [SerializeField] private float _pixelsPerUnitMultiplier = 1f;
        [SerializeField] private Color _color = new Color(0f, 0f, 0f, 0.8f);

        private GameObject _objBG;

        protected override Tween GetTween(View view, float duration)
        {
            view.OnCloseEnd.AddListener(() =>
            {
                Object.Destroy(_objBG);
            });

            // Spawn background
            SpawnBackground(view);

            // Return background fade tween
            Image image = _objBG.GetComponent<Image>();
            image.color = _color;

            if (_sprite != null)
            {
                image.sprite = _sprite;
                image.type = _imageType;
                image.pixelsPerUnitMultiplier = _pixelsPerUnitMultiplier;
            }

            return image.DOFade(_color.a, duration)
                        .ChangeStartValue(new Color(_color.r, _color.g, _color.b, 0.0f))
                        .SetEase(Ease.Linear);
        }

        private void SpawnBackground(View view)
        {
            _objBG = new GameObject($"{view.name} Background");

            RectTransform rect = _objBG.AddComponent<RectTransform>();
            rect.SetParent(view.TransformCached.parent);
            rect.SetSiblingIndex(view.transform.GetSiblingIndex());
            rect.SetScale(1.0f);

            rect.Fill();

            _objBG.AddComponent<Image>();

            if (_closeOnClick)
            {
                Button button = _objBG.AddComponent<Button>();
                button.transition = Selectable.Transition.None;
                button.onClick.AddListener(() => { view.Close(); button.interactable = false; });
            }
        }
    }
}
