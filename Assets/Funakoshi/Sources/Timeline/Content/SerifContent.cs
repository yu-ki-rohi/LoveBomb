using UnityEngine;
using TMPro;

public class TSerifContent : TimelineContent
{
    public TextMeshProUGUI textComponent;

    [SerializeField] string serif = string.Empty;
    [SerializeField] int framesForChar = 1;

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
