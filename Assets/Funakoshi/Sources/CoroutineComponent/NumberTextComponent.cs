using UnityEngine;
using TMPro;
using System.Collections;

public class NumberTextComponent : CoroutineContent
{
    [SerializeField] private TextMeshProUGUI textComponent;

    [SerializeField] private float displayIncreaseSpeed = 1;

    private int numberValue;

    public void InitalSetValue(int value)
    {
        numberValue = value;
    }
    public override void ProcessStarted()
    {
        StartCoroutine(IncreaseAnimation());
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
