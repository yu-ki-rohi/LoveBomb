using UnityEngine;
using UnityEngine.UI;

public class PlayerIndividualData
{
    public PlayerIndividualData(PlayerParameters parameters, Image heartGauge)
    {
        this.parameters = parameters;
        HeartGauge = heartGauge;
    }

    public Player.State State = Player.State.Idle;
    public Transform Transform;
    public Vector2 MoveDir = Vector2.zero;
    public Vector2 ShootDir = Vector2.right;

    // ƒQ[ƒ€ƒpƒbƒh‚ªÚ‘±‚³‚ê‚Ä‚¢‚é‚©
    public bool IsGamePadConnected = false;
    public bool IsRStickInput = false;

    private int heartEnergy = 0;
    private PlayerParameters parameters;
    // ˆê’U‚±‚±‚Å•ÛŽ
    private Image HeartGauge;

    public int HeartEnergy { get => heartEnergy; }

    public bool IsIdle { get => State == Player.State.Idle; }
    public bool IsShooting { get => State == Player.State.Aim || State == Player.State.Shoot; }

    public CameraOffsetByCinemachineParameters CameraOffsetByCinemachineParameters { get => parameters.CameraOffsetParameters; }

    public void ChangeState(Player.State nextState)
    {
        DebugMessenger.Log("State: " + State + " ¨ " + nextState);
        State = nextState;
    }

    public void AddHeartEnergy(int heartEnergy)
    {
        this.heartEnergy += heartEnergy;
        if(this.heartEnergy > parameters.PlayerShootParameters.HeartEnergyMax)
        {
            this.heartEnergy = parameters.PlayerShootParameters.HeartEnergyMax;
        }
        ReflectUI();
    }

    public bool ConsumeHeartEnergy(int cost)
    {
        if(cost > heartEnergy) { return false;}
        heartEnergy -= cost;
        ReflectUI();
        return true;
    }

    public void LoseHeartEnergy(int amount)
    {
        heartEnergy = Mathf.Max(heartEnergy - amount, 0);
        ReflectUI();
    }

    public void ReflectUI()
    {
        HeartGauge.fillAmount = (float)heartEnergy / parameters.PlayerShootParameters.HeartEnergyMax;
    }
}
