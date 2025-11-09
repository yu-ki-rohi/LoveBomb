using UnityEngine;
using System.Collections.Generic;

public class CoreController : MonoBehaviour
{
    [Header("ゲームオーバーになる敵の数")]
    [SerializeField] private int maxEnemyCount = 10;

    [Header("現在コアに触れている敵の数")]
    [SerializeField] private int currentEnemyCount = 0;

    [Header("ゲームオーバーのフラグ")]
    [SerializeField] private bool isGameOver = false;

    // Coreに当たっている敵のリスト
    private readonly List<Rigidbody2D> touchingEnemies = new List<Rigidbody2D>();

    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }

    public int MaxEnemyCount
    {
        get { return maxEnemyCount; }
        set { maxEnemyCount = value; }
    }

    public int CurrentEnemyCount
    {
        get { return currentEnemyCount; }
        set { currentEnemyCount = value; }
    }

    // 敵が当たった瞬間
    private void OnTriggerEnter2D(Collider2D other)
    {
        // タグがEnemyなら、当たっている敵の数を増やす
        if (other.CompareTag("Enemy"))
        {
            // EnemyのRigidbody2Dを取得
            Rigidbody2D enemyRb = other.attachedRigidbody;

            // EnemyのRigidbody2Dがnullでないかつ、当たった敵がリストに入っていない場合（重複防止）
            if (enemyRb != null && touchingEnemies.Contains(enemyRb) == false)
            {
                // リストに追加し、リストの数で管理
                touchingEnemies.Add(enemyRb);
                AudioManager.Instance.PlaySEById(SEName.Noroi);
                currentEnemyCount = touchingEnemies.Count;
            }
        }

        // ゲームオーバーになる敵の数以上になったら、フラグを立てる
        if (currentEnemyCount >= maxEnemyCount)
        {
            isGameOver = true;
        }
    }

    // 敵が離れた瞬間
    private void OnTriggerExit2D(Collider2D other)
    {
        // タグがEnemyなら、当たっている敵の数を減らす
        if (other.CompareTag("Enemy"))
        {
            // EnemyのRigidbody2Dを取得
            Rigidbody2D enemyRb = other.attachedRigidbody;

            //  EnemyのRigidbody2Dがnullでないかつ、当たった敵がリストに入っている場合
            if (enemyRb != null && touchingEnemies.Contains(enemyRb) == true)
            {
                // リストから削除し、リストの数で管理
                touchingEnemies.Remove(enemyRb);
                currentEnemyCount = touchingEnemies.Count;
            }
        }
    }
}
