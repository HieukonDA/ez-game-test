using UnityEngine;

public class EnemyVictoryState : StateBase
{
    public EnemyVictoryState(EnemyAI enemyAI) : base(enemyAI) { }
    protected override string GetAnimationName() => "Victory";
}