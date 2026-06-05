using System.Collections.Generic;
using UnityEngine;

// 基本的なエフェクトのプール
public class EffectPoolManager : PoolManager<PooledEffect>
{
    [SerializeField] private EffectList list;
    private Dictionary<EffectData.EffectType, EffectData> effectDataBase;

    public PooledEffect PlayEffect(Vector3 position, EffectData.EffectType type, float speed = 1.0f)
    {
        if (DebugMessenger.NullCheckError(objectPool)) { return null; }
        var effect = objectPool.Get();

        effect.transform.position = position;
        effect.transform.localScale = new Vector3(effectDataBase[type].Scale, effectDataBase[type].Scale, 1.0f);
        effect.Initialize(effectDataBase[type], speed);

        return effect;
    }

    protected override void Awake()
    {
        base.Awake();
        if (DebugMessenger.NullCheckError(list)) { return; }
        effectDataBase = new Dictionary<EffectData.EffectType, EffectData>();
        foreach(var effectData in list.Effects)
        {
            effectDataBase[effectData.Type] = effectData;
        }
    }

}
