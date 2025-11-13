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
    public int HeartEnergy = 0;
    public Transform Transform;
    public Vector2 MoveDir = Vector2.zero;
    private PlayerParameters parameters;
    // ˆê’U‚±‚±‚Å•ÛŽ
    private Image HeartGauge;

    public bool IsShooting { get => State == Player.State.Aim || State == Player.State.Shoot; }

    public void ChangeState(Player.State nextState)
    {
        DebugMessenger.Log("State: " + State + " ¨ " + nextState);
        State = nextState;
    }

    public void AddHeartEnergy(int heartEnergy)
    {
        HeartEnergy += heartEnergy;
        if(HeartEnergy > parameters.PlayerShootParameters.HeartEnergyMax)
        {
            HeartEnergy = parameters.PlayerShootParameters.HeartEnergyMax;
        }
        ReflectUI();
    }

    public bool ConsumeHeartEnergy(int cost)
    {
        if(cost > HeartEnergy) { return false;}
        HeartEnergy -= cost;
        ReflectUI();
        return true;
    }

    public void ReflectUI()
    {
        HeartGauge.fillAmount = (float)HeartEnergy / parameters.PlayerShootParameters.HeartEnergyMax;
    }
}
