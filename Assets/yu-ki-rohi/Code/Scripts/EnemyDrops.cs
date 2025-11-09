using UnityEngine;

public class EnemyDrops : MonoBehaviour, ICoreChargable
{
    private Transform coreTransform;
    [SerializeField] GameObject dropItemParticleSystem;

    void Start()
    {
        EnemyDropParticle.Instantiate(dropItemParticleSystem, transform.position, coreTransform, this);
    }

    public void ChargeExecute()
    {
        AudioManager.Instance.PlaySEById(SEName.Charge);
    }

    public void SetTarget(Transform target)
    {
        coreTransform = target;
    }
}
