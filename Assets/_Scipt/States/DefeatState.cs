using Unity.VisualScripting;
using UnityEngine;

public class DefeatState : StateBase
{
    public DefeatState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "Defeat";
}