using UnityEngine;

public class BlockState : StateBase
{
    public BlockState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "Block";
}