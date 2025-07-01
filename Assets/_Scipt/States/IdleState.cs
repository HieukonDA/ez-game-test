using UnityEngine;

public class IdleState : StateBase
{
    public IdleState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "Idle";
}