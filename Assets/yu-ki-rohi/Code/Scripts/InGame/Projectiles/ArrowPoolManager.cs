using System.Security.Cryptography;
using UnityEngine;

public class ArrowPoolManager : PoolManager<Arrow>
{
    [SerializeField] private ExplosionPoolManager explosionPoolManager;
    private PlayerShootParameters parametersOfArrows;

    public int GetCost(Arrow.Type arrowType)
    {
        foreach (var arrowParams in parametersOfArrows.Arrows)
        {
            if (arrowParams.ArrowType == arrowType)
            {
                return arrowParams.Cost;
            }
        }
        return 0;
    }

    public void SetArrowParameters(PlayerShootParameters parameters)
    {
        parametersOfArrows = parameters;
    }

    public void Shoot(Vector3 position, Vector2 shootDir, Arrow.Type arrowType)
    {
        if(objectPool == null)
        {
            Debug.LogError("ObjectPool is Null !!");
            return;
        }

        var arrow = objectPool.Get();

        if(arrow == null)
        {
            Debug.Log("Fail to get Arrow Object");
            return;
        }

        arrow.transform.position = position;
        arrow.transform.up = shootDir;
        arrow.Initialize(arrowType);
    }

    protected override Arrow Create()
    {
        var arrow = base.Create();
        arrow.ParametersOfParameters = parametersOfArrows;
        arrow.ExplosionPoolManager = explosionPoolManager;
        return arrow;
    }
}
