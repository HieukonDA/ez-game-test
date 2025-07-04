using UnityEngine;

public class EnemyKnockedOutState : StateBase
{
    public EnemyKnockedOutState(EnemyAI enemyAI) : base(enemyAI) { }
    protected override string GetAnimationName() => "KnockedOut";
}