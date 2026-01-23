using UnityEngine;

[CreateAssetMenu(fileName = "EffectList", menuName = "EffectList")]
public class FadeComponent : ScriptableObject
{
    public Animator Animator;

    public string FadeOutTrigger = "FadeOut";

    public string FadeInTrigger = "FadeIn";
}
