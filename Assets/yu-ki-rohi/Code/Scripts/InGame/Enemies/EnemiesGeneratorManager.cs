using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ジェネレーターに攻撃対象を登録するために用意したクラス
//
public class EnemiesGeneratorManager : MonoBehaviour
{
    // todo: 下記をHideInInspectorに *今は確認のためインスペクターに表示
    [SerializeField] private List<EnemiesGenerator> enemyGenerators;
    [SerializeField] private List<GeneratorBase> generators;

    [SerializeField] private Transform core;
    [SerializeField] private ManagedEnemyPoolManager pool;

    public void SetTargetToGenerators(Transform player)
    {
        foreach (var generator in enemyGenerators)
        {
            if(DebugMessenger.NullCheckError(generator) || DebugMessenger.NullCheckError(generator.EnemyData)) { continue; }
            if( generator.EnemyData.Type == Enemy.Type.ChasePlayer )
            {
                generator.Target = player;
            }
            else
            {
                generator.Target = core;
            }
        }
    }

    public void BootGenerators()
    {
        foreach (var generator in generators)
        {
            generator.BootGenerateAsync();
        }
    }


#if UNITY_EDITOR
    public void SetGenerators()
    {
        enemyGenerators.Clear();
        generators.Clear();

        var enemiesGeneratorArray = FindObjectsByType<EnemiesGenerator>(FindObjectsSortMode.None);

        foreach(var enemiesGenerator  in enemiesGeneratorArray)
        {
            enemyGenerators.Add(enemiesGenerator);
            var generatorBase = enemiesGenerator.gameObject.GetComponent<GeneratorBase>();
            if(generatorBase != null )
            {
                generators.Add(generatorBase);
            }
        }
        EditorUtility.SetDirty(this);
    }

    public void SetIsBootOnStart(bool isBoot)
    {
        foreach (var generator in generators)
        {
            generator.IsBootOnStart = isBoot;
            EditorUtility.SetDirty(generator);
        }
    }

    public void SetPooToAllObjectInThisScene()
    {
        if (pool == null)
        {
            Debug.LogWarning("Enemy Pool is Null");
        }
        Debug.Log("Start to set Enemy Pool to All GameObject");

        // IEnemyGeneratorを持つオブジェクトを取得
        var generators = FindObjectsByType<GeneratorBase>(FindObjectsSortMode.None).OfType<EnemiesGenerator>();
        foreach (var generator in generators)
        {
            generator.SetPool(pool);
            EditorUtility.SetDirty(generator);
        }
    }

#endif
}
