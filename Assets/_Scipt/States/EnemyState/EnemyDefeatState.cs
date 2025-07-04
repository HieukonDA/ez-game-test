using UnityEngine;

public class EnemyDefeatState : StateBase
{
    public EnemyDefeatState(EnemyAI enemyAI) : base(enemyAI) { }
    protected override string GetAnimationName() => "Defeat";
}