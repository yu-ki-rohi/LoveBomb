using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerComponentBase
{
    protected readonly PlayerIndividualData player;

    public PlayerComponentBase(PlayerIndividualData ownersData)
    {
        player = ownersData;
    }
}
