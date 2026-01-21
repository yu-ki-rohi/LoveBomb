using UnityEngine;

public class CommonDropItemPoolManger : PoolManager<CommonDropItem>
{
    [SerializeField] ItemDataBase dataBase;
    public void DropItem(int id, Vector3 position)
    {
        if(id < 0 || id >= dataBase.Items.Count) { return; }
        var dropItem = objectPool.Get();
        dropItem.Initialize(id, dataBase.Items[id]);
        dropItem.transform.position = position;
    }

    protected override CommonDropItem Create()
    {
        var instanse = base.Create();
        instanse.OnCreated(dataBase.CommonData);
        return instanse;
    }
}
