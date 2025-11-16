using UnityEngine;

public class HeartEnergyPoolManager : PoolManager<HeartEnergy>
{
    [SerializeField] ItemDataBase dataBase;
    public void GenerateHeart(int energy, Vector3 position)
    {
        var heartEnergy = objectPool.Get();
        heartEnergy.Initialize(energy);
        heartEnergy.transform.position = position;
    }

    protected override HeartEnergy Create()
    {
        var instanse = base.Create();
        instanse.OnCreated(dataBase.CommonData);
        return instanse;
    }
}
