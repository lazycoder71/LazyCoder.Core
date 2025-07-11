using UnityEngine;

namespace LazyCoder.Core
{
    public static class ExtensionsSpriteRenderer
    {
        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            Color newColor = spriteRenderer.color;
            newColor = new Color(newColor.r, newColor.g, newColor.b, alpha);

            spriteRenderer.color = newColor;
        }
    }
}