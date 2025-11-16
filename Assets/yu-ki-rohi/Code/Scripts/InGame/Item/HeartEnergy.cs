using UnityEngine.Pool;

public class HeartEnergy : DropItemBase,IPooledObject<HeartEnergy>
{
    private IObjectPool<HeartEnergy> pool;
    private int energy;

    public IObjectPool<HeartEnergy> ObjectPool { set { pool = value; } }

    public void Initialize(int energy)
    {
        this.energy = energy;
        base.Initialize();
    }


    public override void Deactivate()
    {
        pool.Release(this);
    }

    public int GainEnergy()
    {
        Deactivate();
        return energy;
    }

    protected override void OnReachPlayer()
    {
        target.AddHeartEnergy(energy);
    }

}
