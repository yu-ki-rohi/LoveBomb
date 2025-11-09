using UnityEngine;
using UnityEngine.UI;

public class ColorAlpha
{
    private readonly Image targetImage;

    public ColorAlpha(Image image)
    {
        targetImage = image;
    }

    public void Set(float value)
    {
        Color setColor = targetImage.color;
        setColor.a = value;
        targetImage.color = setColor;
    }
    public void Increase(float value)
    {
        float setAlpha = targetImage.color.a + value;
        setAlpha = Mathf.Clamp01(setAlpha);
        Set(setAlpha);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="speed"></param>
    /// <returns>“§–¾“x‚ª–Ú“I‚É“ž’B‚µ‚Ä‚¢‚é‚©‚Ç‚¤‚©‚ð•Ô‚µ‚Ü‚·</returns>
    public bool ToWard(float destination, float speed)
    {
        float currentAlpha = targetImage.color.a;
        if (Mathf.Abs(destination - currentAlpha) < Mathf.Abs(speed))
        {
            Set(destination);
            return true;
        }
        else
        {
            currentAlpha += speed;
            Set(currentAlpha);
            return false;
        }
    }
}
