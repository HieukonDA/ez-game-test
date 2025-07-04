using UnityEngine;

public class EnemyBlockState : StateBase
{
    public EnemyBlockState(EnemyAI enemyAI) : base(enemyAI) { }
    protected override string GetAnimationName() => "Block";
}