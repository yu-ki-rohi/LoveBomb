using UnityEngine;

[CreateAssetMenu(fileName = "ParticleSettings", menuName = "Scriptable Objects/ParticleSettings")]
public class ParticleSettings : ScriptableObject
{
    [Header("生成するパーティクル数")]
    public int particleCount = 4;

    [Header("パーティクルの大きさ")]
    public float particleSize = 1;

    [Header("放射の強さ")]
    public float emissionIntensity = 1.2f;

    [Header("放射フェーズの時間")]
    public float emitDuration = 0.5f;

    // yu-ki-rohi追加
    // Range属性
    [Header("放射時の減速(0.5～0.99)"), Range(0.5f, 0.99f)]
    public float emittingSpeed​Multiplier = 0.9f;

    [Header("滞留フェーズの時間")]
    public float stayDuration = 1f;

    [Header("Coreへ向かうフェーズの時間")]
    public float moveDuration = 1f;

    [Header("打ち上げられる強さ")]
    public float launchPower;

    [Header("Coreへ向かう測度")]
    public float moveAccelaration = 5f;
    public float moveMaxSpeed = 5f;
}
