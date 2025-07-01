using UnityEngine;

public class LeftUpperCutState : StateBase
{
    public LeftUpperCutState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "LeftUpperCut";

}