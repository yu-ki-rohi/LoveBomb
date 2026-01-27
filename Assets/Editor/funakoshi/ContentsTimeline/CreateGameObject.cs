using UnityEngine;

public record TimelineContentFactory
{
    public TimelineContent CreateGameObject(TimelineNodeType type)
    {
        var gameObject = new GameObject(type.ToString());

        switch (type)
        {
            case TimelineNodeType.Serif:
                var content = gameObject.AddComponent<TSerifContent>();
                return content;
            default:
                throw new System.NotImplementedException($"type : {type} ÇÃÉPÅ[ÉXÇ™é¿ëïÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
        }
    }
}
