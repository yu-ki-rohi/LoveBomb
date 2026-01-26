using UnityEngine;

public class UsedItemPoolManager : PoolManager<UsedItem>
{
    private ExplosionPoolManager explosionPoolManager;
    
    public ExplosionPoolManager ExplosionPoolManager {  set {  explosionPoolManager = value; } }

    public void UseItem(ItemData data, Vector3 position, float Scale)
    {
        var item = objectPool.Get();

        item.Initialize(data);
        item.transform.position = position;
        item.transform.localScale = new Vector3(Scale, Scale, 1);
    }

    protected override UsedItem Create()
    {
        var instanse = base.Create();
        instanse.OnCreated(explosionPoolManager);
        return instanse;
    }
}