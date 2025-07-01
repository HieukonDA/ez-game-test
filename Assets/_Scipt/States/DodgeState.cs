using UnityEngine;

public class DodgeState : StateBase
{
    public DodgeState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "Dodge";

}