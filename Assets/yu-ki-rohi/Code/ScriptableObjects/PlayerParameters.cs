using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerMovementParameters
{
    [Header("移動速度関連"), Min(1.0f)]
    public float Agility = 10.0f;
    [Range(1.0f, 3.0f)]
    public float DashSpeedMultiplier = 1.2f;
    [Range(0.1f, 1.0f)]
    public float AimSpeedMultiplier = 0.8f;

    [Header("MovePatternがAddForce系列時のみ使用")]
    [Range(1.0f, 60.0f)]
    public float AccelerationForce = 30.0f;
    [Range(0.0f, 20.0f)]
    public float DampingForce = 10.0f;

    [Header("AddForceAndEscapeで使用")]
    [Range(30.0f, 120.0f)]
    public float BurstForce = 50.0f;
    [Range(0.5f, 5.0f)]
    public float BurstCoolTime = 1.0f;


    [Header("画面端に行った時のバッファ"), Min(0.0f)]
    public float PlayerRadius = 0.5f;

}

[Serializable]
public class PlayerAnimationParameters
{
    [Header("射撃アニメーション時間")]
    [Range(0.01f, 0.5f)]
    public float LeadInTime = 0.1f;
    [Range(0.01f, 0.5f)]
    public float FollowThroughTime = 0.3f;
}

[Serializable]
public class CameraOffsetParameters
{

    [Min(0.0f)]
    public float OffsetX = 5.0f;
    [Min(0.0f)]
    public float OffsetY = 3.0f;
    [Min(0.0f)]
    public float ChangeOffsetTime = 1.0f;
}

[Serializable]
public class ArrowParams
{
    public string Name;
    public Arrow.Type ArrowType = Arrow.Type.Normal;

    public Sprite Image;

    [Min(0)]
    public int Power = 1;
     [Min(0)]
    public int Cost = 1;

    [Min(0.0f)]
    public float Speed = 15.0f;
    [Min(0.0f)]
    public float EffectiveRange = 10.0f;
}

[Serializable]
public class PlayerShootParameters
{
    [Range(0.0f, 2.0f)]
    public float ShootPositionDistance = 0.5f;
    [Range(0.1f, 5.0f)]
    public float ChargeTime = 1.0f;
    [Header("各種矢の設定")]
    public List<ArrowParams> Arrows;

    [Range(0.1f, 5.0f)]
    public float ExplosionScale = 1.0f;
    
    [Min(1)]
    public int HeartEnergyMax = 1;

    [Min(1)]
    public int InitialHeartEnergy = 1;
}


[CreateAssetMenu(fileName = "PlayerParameters", menuName = "CharacterData/PlayerParameters")]
public class PlayerParameters : ScriptableObject
{
    [Header("移動関連設定")]
    public PlayerMovementParameters PlayerMovementParameters;

    [Header("モーション速度関連設定")]
    public PlayerAnimationParameters PlayerAnimationParameters;

    [Header("Cameraのoffset関係設定")]
    public CameraOffsetParameters CameraOffsetParameters;

    [Header("射撃関連設定")]
    public PlayerShootParameters PlayerShootParameters;

}
