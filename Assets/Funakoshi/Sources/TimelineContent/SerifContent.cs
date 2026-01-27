using System.Collections;
using UnityEngine;
using TMPro;

public class TSerifContent : TimelineContent
{
    public TextMeshProUGUI textComponent;
    public string serif;
    public int framesForChar;

    private TextUseCase text;

    void Awake()
    {
        text = new TextUseCase(textComponent);
    }

    public override float Duration()
    {
        return serif.Length * framesForChar;
    }
    protected override void BeforeBegin()
    {
        text.ClearText();
    }
    protected override void Evalute(float localTime)
    {
        if (localTime < 0)
        {
            text.ClearText(); 
            return;
        }

    }
    protected override void AfterFinished()
    {
        text.SetText(serif);
    }
}
