using UnityEngine;

public class KnockedOutState : StateBase
{
    public KnockedOutState(PlayerController playerController) : base(playerController) { }
    protected override string GetAnimationName() => "KnockedOut";
}