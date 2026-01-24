using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TransparentImage
{
    public Image image;

    public void NullCheck()
    {
        if (image == null)
            throw new NullReferenceException("Imageがアタッチされていません");
    }

    public void SetAlpha(float alpha)
    {
        NullCheck();

        alpha = Mathf.Clamp01(alpha);

        Color currentColor = image.color;

        currentColor.a = alpha;

        image.color = currentColor;
    }
}