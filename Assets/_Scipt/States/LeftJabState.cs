using UnityEngine;

public class LeftJabState : StateBase
{
    public LeftJabState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "LeftJab";

}