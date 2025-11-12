using UnityEngine;

public class PlayerIndividualData
{
    public PlayerIndividualData(PlayerParameters parameters)
    {
        this.parameters = parameters;
    }

    public Player.State State = Player.State.Idle;
    public int HeartEnergy = 0;
    public Transform Transform;
    public Vector2 MoveDir = Vector2.zero;
    private PlayerParameters parameters;

    public bool IsShooting { get => State == Player.State.Aim || State == Player.State.Shoot; }

    public void ChangeState(Player.State nextState)
    {
        DebugMessenger.Log("State: " + State + " Å® " + nextState);
        State = nextState;
    }
}
