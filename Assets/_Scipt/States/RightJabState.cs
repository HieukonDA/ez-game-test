using UnityEngine;

public class RightJabState : StateBase
{
    public RightJabState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "RightJab";

}