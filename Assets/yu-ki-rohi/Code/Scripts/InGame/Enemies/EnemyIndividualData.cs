using UnityEngine;

// エネミー個々のデータ
public class EnemyIndividualData
{
    public EnemyData BasicData;
    public HeartCore HeartCore;

    // 自身がつながっているエネミー
    public Enemy HoldingHandsEnemy;

    public int CurrentHitPoint = 0;
    public Vector3 MoveDir = Vector3.zero;

    // 自身につながっているエネミーの数
    public int ConcatenatingNum = 0;
    public Animator Animator;
    public Rigidbody2D Rigidbody;
}
