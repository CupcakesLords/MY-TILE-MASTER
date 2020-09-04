using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AFramework.ExtensionMethods
{
    public static class ImageExtension
    {
        public static void UpdatePivot(this Image img, float scale)
        {
            img.rectTransform.pivot = new Vector2(img.sprite.pivot.x / img.sprite.rect.width,
                img.sprite.pivot.y / img.sprite.rect.height);
            img.rectTransform.anchoredPosition = Vector2.zero;
            img.rectTransform.sizeDelta = img.sprite.rect.size * scale;
        }
    }
}