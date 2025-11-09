using TMPro;

public class TextUseCase
{
    public TextMeshProUGUI textComponent;

    public TextUseCase(TextMeshProUGUI textComponent)
    {
        this.textComponent = textComponent;
    }

    public void ClearText()
    {
        textComponent.text = string.Empty;
    }
    public void SetText(string text)
    {
        textComponent.text = text;
    }
    public void AddNewCharToText(char addText)
    {
        textComponent.text += addText;
    }
}
