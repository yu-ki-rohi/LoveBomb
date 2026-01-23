using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TransparentImage
{
    public Image image;

    public void SetAlpha(float alpha)
    {
        if (image == null)
            throw new NullReferenceException("Imageがアタッチされていません");

        alpha = Mathf.Clamp01(alpha);

        Color currentColor = image.color;

        currentColor.a = alpha;

        image.color = currentColor;
    }
}