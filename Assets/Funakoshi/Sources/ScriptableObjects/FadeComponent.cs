using UnityEngine;

[CreateAssetMenu(fileName = "FadeComponent", menuName = "ScriptableObjects/FadeComponent")]
public class FadeComponent : ScriptableObject
{
    public Animator Animator;

    public string FadeOutTrigger = "FadeOut";

    public string FadeInTrigger = "FadeIn";
}
