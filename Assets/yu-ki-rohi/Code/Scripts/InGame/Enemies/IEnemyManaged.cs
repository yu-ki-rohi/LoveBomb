using UnityEngine;
// EnemyManagerから動かすためのインターフェース
// 意図としてはアクセス制限的な
public interface IEnemyManaged
{
    // 戻り値として、Updateが正常終了したかどうかを返す
    public bool ManagedUpdate();

    public bool ManagedFixedUpdate();
}
