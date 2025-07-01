using UnityEngine;

public class RightUpperCutState : StateBase
{
   public RightUpperCutState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "RightUpperCut";

}