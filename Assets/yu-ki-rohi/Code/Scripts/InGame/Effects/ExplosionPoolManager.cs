using UnityEngine;

public class ExplosionPoolManager : PoolManager<Explosion>
{
    public void Explode(int power, Vector3 position, float scaleMultiplier)
    {
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool is Null !!");
            return;
        }

        var explosion = objectPool.Get();

        if (explosion == null)
        {
            Debug.Log("Fail to get Explosion Object");
            return;
        }
        explosion.transform.position = position;
        explosion.transform.localScale = new Vector3 (scaleMultiplier, scaleMultiplier, 1.0f);  // zÇÕä÷åWÇ»Ç¢ÇÃÇ≈ÅAÇ∆ÇËÇ†Ç¶Ç∏1Ç≈
        explosion.Initialize(power);
    }
}
