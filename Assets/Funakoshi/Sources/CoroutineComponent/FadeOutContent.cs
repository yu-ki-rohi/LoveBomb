using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutContent : CoroutineContent
{
    [SerializeField] private Image thisImage;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float terminalAlpha;

    private ColorAlpha colorAlpha;

    void Awake()
    {
        colorAlpha = new ColorAlpha(thisImage);
        if (thisImage == null)
            Debug.LogError("Imageがアタッチされていません");
    }

    public override void ProcessStarted()
    {
        StartCoroutine(Fade());
    }
    public override void ForcedEnd()
    {
        StopAllCoroutines();
        colorAlpha.Set(terminalAlpha);

        contentEnd = true;
    }

    private IEnumerator Fade()
    {
        while (!colorAlpha.ToWard(terminalAlpha, fadeSpeed))
        {
            yield return new WaitForFixedUpdate();
        }

        contentEnd = true;
    }
}
