using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(SpriteRenderer))]
public class CommonDropItem : DropItemBase, IPooledObject<CommonDropItem>
{
    private IObjectPool<CommonDropItem> pool;
    private int id;

    public IObjectPool<CommonDropItem> ObjectPool { set { pool = value; } }

    public void Initialize(int id, ItemData data)
    {
        this.id = id;
        this.data = data;
        spriteRenderer.sprite = data.Icon;
        base.Initialize();
    }


    public override void Deactivate()
    {
        pool.Release(this);
    }

    protected override void OnReachPlayer()
    {
        target.AddItem(id);
    }

}
