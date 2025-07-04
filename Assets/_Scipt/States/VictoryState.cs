using UnityEngine;

public class VictoryState : StateBase
{
    public VictoryState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "Victory";
}