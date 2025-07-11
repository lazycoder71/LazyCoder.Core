using UnityEngine;

namespace LazyCoder.Core
{
    public static class ExtensionsRectTransform
    {
        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
        {
            rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
        }

        public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
        }

        public static void TranslateX(this RectTransform rectTransform, float x)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + x, rectTransform.anchoredPosition.y);
        }

        public static void TranslateY(this RectTransform rectTransform, float y)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + y);
        }

        public static void TranslateXY(this RectTransform rectTransform, float x, float y)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + x, rectTransform.anchoredPosition.y + y);
        }

        public static void Fill(this RectTransform rectTransform)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.rotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }

        public static float GetPixelPerUnit(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];

            rectTransform.GetWorldCorners(corners);

            return rectTransform.rect.width / (corners[2].x - corners[0].x);
        }

        public static float GetUnitPerPixel(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];

            rectTransform.GetWorldCorners(corners);

            return (corners[2].x - corners[0].x) / rectTransform.rect.width;
        }

        public static Vector3 ToWorldPosition(this RectTransform rectTransform, Camera camera = null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, rectTransform.position, camera, out var local);
            var world = rectTransform.TransformPoint(new Vector3(local.x, local.y));
            return world;
        }
    }
}