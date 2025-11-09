using UnityEngine;

public interface IEnemyPoolManager
{
    public void EnemyAppear(Vector3 position, Transform target, EnemyData data);
#if UNITY_EDITOR
    public EnemyDataList EnemyDataList { get; }
#endif
}
