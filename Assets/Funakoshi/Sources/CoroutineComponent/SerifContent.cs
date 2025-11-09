using System.Collections;
using UnityEngine;
using TMPro;

public class SerifContent : CoroutineContent
{
    [SerializeField] TextMeshProUGUI textComponent;
    [SerializeField] private string hereMessage;
    [SerializeField] private int framesForChar;

    private TextUseCase text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        text = new TextUseCase(textComponent);
    }
    public override void ProcessStarted()
    {
        text.ClearText();
        StartCoroutine(IncreaseExecute(hereMessage));
    }
    public override void ForcedEnd()
    {
        StopAllCoroutines();
        text.SetText(hereMessage);
        contentEnd = true;
    }

    private IEnumerator IncreaseExecute(string message)
    {
        hereMessage = message;

        int messageLength = message.Length;
        int viewCharsCount = 0;
        while (true)
        {
            if (messageLength <= viewCharsCount)
            {
                break;
            }

            text.AddNewCharToText(message[viewCharsCount]);

            for (int i = 0; i < framesForChar; i++)
                yield return new WaitForFixedUpdate();

            viewCharsCount++;
        }
    }
}
