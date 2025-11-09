using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerComponentBase
{
    protected readonly Player player;

    public PlayerComponentBase(Player owner)
    {
        player = owner;
    }
}
