using UnityEngine;

public class EnemyIdleState : StateBase
{
    public EnemyIdleState(EnemyAI enemyAI) : base(enemyAI) { }
    protected override string GetAnimationName() => "Idle";
}