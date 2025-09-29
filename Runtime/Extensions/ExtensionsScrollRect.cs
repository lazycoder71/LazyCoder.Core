using UnityEngine;
using UnityEngine.UI;

namespace LazyCoder.Core
{
    public static class ExtensionsScrollRect
    {
        public static void ScrollTo(this ScrollRect scrollRect, Transform target, bool isVertical = true)
        {
            if (isVertical)
            {
                scrollRect.normalizedPosition = new Vector2(0f,
                    1f - (scrollRect.content.rect.height * 0.5f - target.localPosition.y) /
                    scrollRect.content.rect.height);
            }
            else
            {
                scrollRect.normalizedPosition =
                    new Vector2(
                        1f - (scrollRect.content.rect.width * 0.5f - target.localPosition.x) /
                        scrollRect.content.rect.width, 0f);
            }
        }
    }
}