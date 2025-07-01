using UnityEngine;

public class RightHookState : StateBase
{
    public RightHookState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "RightHook";

}