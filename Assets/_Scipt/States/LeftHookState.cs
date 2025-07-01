using UnityEngine;
using UnityEngine.Rendering;

public class LeftHookState : StateBase
{
    public LeftHookState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "LeftHook";


}