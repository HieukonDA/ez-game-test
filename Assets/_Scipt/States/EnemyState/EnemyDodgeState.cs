using UnityEngine;

public class EnemyDodgeState : StateBase
{
    public EnemyDodgeState(EnemyAI enemyAI) : base(enemyAI) { }
    protected override string GetAnimationName() => "Dodge";
}