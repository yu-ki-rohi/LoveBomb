using UnityEngine;
using TMPro;
using System.Collections;

public class NumberTextComponent : CoroutineContent
{
    [SerializeField] private TextMeshProUGUI textComponent;

    [SerializeField] private float displayIncreaseSpeed = 1;

    [SerializeField] private bool isDisplayAtOnce = false;

    private int numberValue;

    public void InitalSetValue(int value, Color color)
    {
        numberValue = value;
        textComponent.color = color;
        if (isDisplayAtOnce)
        {
            textComponent.text = numberValue.ToString();
        }
    }

    public override void ProcessStarted()
    {
        if(isDisplayAtOnce)
        {
            textComponent.text = numberValue.ToString();
            contentEnd = true;
        }
        else
        {
            StartCoroutine(IncreaseAnimation());
        }
    }
    public override void ForcedEnd()
    {
        StopAllCoroutines();
        textComponent.text = numberValue.ToString();

        contentEnd = true;
    }

    IEnumerator IncreaseAnimation()
    {
        int frames = numberValue / (int)displayIncreaseSpeed;

        float displayNum = 0;
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForFixedUpdate();

            displayNum += displayIncreaseSpeed;
            textComponent.text = ((int)displayNum).ToString();
        }
        textComponent.text = numberValue.ToString();

        contentEnd = true;
    }
}
