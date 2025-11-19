using UnityEngine;

public class HeartEnergyGenerator : Generator
{
    [SerializeField] private HeartEnergyPoolManager poolManager;
    [SerializeField, Min(1)] private int energyAmount = 5;

    void Start()
    {
        if(DebugMessenger.NullCheckError(generator))
        {
            gameObject.SetActive(false);
            return;
        }
        if(DebugMessenger.NullCheckError(poolManager))
        {
            gameObject.SetActive(false);
            return;
        }
        generator.RegisterCallback(OnGenerate);
    }

    protected override void OnGenerate()
    {
        int num = generateNumAtOnce + Random.Range(-generateNumRange, generateNumRange);
        for (int i = 0; i < num; i++)
        {
            poolManager.GenerateHeart(energyAmount, generator.DecideGeneratePosition());
        }
    }

}
