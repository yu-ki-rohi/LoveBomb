using UnityEngine;

public class AnxietyPropagationEffectPoolManager : PoolManager<AnxietyPropagationEffect>
{
    [SerializeField] private EnemyCommonData enemyCommonData;

    public void AnxietyPopagate(Enemy targetEnemy, Vector3 position, int power)
    {
        if (DebugMessenger.NullCheckError(objectPool)) { return; }

        var anxietyPropagationEffect = objectPool.Get();

        if(DebugMessenger.NullCheckError(anxietyPropagationEffect)) { return; }

        anxietyPropagationEffect.Initialize(targetEnemy, position, power);

    }

    protected override AnxietyPropagationEffect Create()
    {
        var instance = base.Create();
        instance.OnCreate(enemyCommonData);
        return instance;
    }
}
