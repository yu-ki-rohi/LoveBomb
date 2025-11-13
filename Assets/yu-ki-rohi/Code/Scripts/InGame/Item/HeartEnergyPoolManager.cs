using UnityEngine;

public class HeartEnergyPoolManager : PoolManager<HeartEnergy>
{
    public void GenerateHeart(int energy, Vector3 position)
    {
        var heartEnergy = objectPool.Get();
        heartEnergy.Initialize(energy);
        heartEnergy.transform.position = position;
    }
}
