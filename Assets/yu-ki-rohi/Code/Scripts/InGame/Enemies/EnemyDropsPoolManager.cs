using UnityEngine;
using System.Collections.Generic;

public class EnemyDropsPoolManager : PoolManager<PooledEnemyDrops>
{
    [SerializeField] private List<HeartCore> heartCores;

    public void DropEnergy(int score, Vector3 position)
    {
        HeartCore targetCore = null;
        float sqrLength = 0.0f;
        foreach(HeartCore heartCore in heartCores)
        {
            if(heartCore == null) { continue; }

            float tempSqrLength = (position - heartCore.transform.position).sqrMagnitude;
            if( targetCore == null || 
                tempSqrLength < sqrLength)
            {
                targetCore = heartCore;
            }
        }
        if(DebugMessenger.NullCheckError(targetCore,"You should check Enemy Drop Pool Manager > HeartCores")) { return; }
        var drop = objectPool.Get();
        drop.Initialize(targetCore, score);
        drop.transform.position = position;
    }
}
