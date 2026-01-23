using UnityEngine;

public class EnemyItemDropTest : MonoBehaviour, ICoreChargable
{
    [SerializeField] Transform coreTransform;
    [SerializeField] GameObject dropItemParticleSystem;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        EnemyDropParticle.Instantiate(dropItemParticleSystem, new Vector3(7, -3), coreTransform, this);
    }
    public void ChargeExecute()
    {
        Debug.Log("ChargeExecute()");
    }
}
