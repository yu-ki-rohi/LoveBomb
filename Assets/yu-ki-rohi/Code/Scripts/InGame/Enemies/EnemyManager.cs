using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private int enemyNumMax = 500;
    private List<IEnemyManaged> managedEnemies = new List<IEnemyManaged>();

    public int EnemyNum { get => managedEnemies.Count; }

    public bool IsEnemyMax { get => managedEnemies.Count >= enemyNumMax; }

    public void AddManagedEnemy(IEnemyManaged enemy)
    {
        managedEnemies.Add(enemy);
    }

    public void RemoveManagedEnemy(IEnemyManaged enemy)
    {
        managedEnemies.Remove(enemy);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var enemy in managedEnemies)
        {
            if (enemy == null) { continue; }

            enemy.ManagedUpdate();
        }
    }

    private void FixedUpdate()
    {
        foreach (var enemy in managedEnemies)
        {
            if (enemy == null) { continue; }

            enemy.ManagedFixedUpdate();
        }
    }
}
