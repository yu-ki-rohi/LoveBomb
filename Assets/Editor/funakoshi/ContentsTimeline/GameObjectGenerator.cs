using TMPro;
using UnityEngine;

public record TimelineGameObjectGenerator
{
    public TimelineContent CreateGameObject(TimelineNodeType type)
    {
        var gameObject = new GameObject(type.ToString());

        switch (type)
        {
            case TimelineNodeType.Serif:
                gameObject.AddComponent<RectTransform>();
                var textComponent = gameObject.AddComponent<TextMeshProUGUI>();
                var content = gameObject.AddComponent<TSerifContent>();
                content.textComponent = textComponent;
                return content;
            default:
                throw new System.NotImplementedException($"type : {type} ÇÃÉPÅ[ÉXÇ™é¿ëïÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
        }
    }
}
