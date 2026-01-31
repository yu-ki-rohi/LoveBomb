using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EffectData
{
    public enum EffectType
    { 
        Charge,
        ChargeEnd,
        HitEffect
    }

    public string Name;
    public EffectType Type;
    public RuntimeAnimatorController Controller;
    public bool IsLoop = false;
    public float Scale = 1.0f;
}

[CreateAssetMenu(fileName = "EffectList", menuName = "EffectList")]
public class EffectList : ScriptableObject
{
    public List<EffectData> Effects;
}
