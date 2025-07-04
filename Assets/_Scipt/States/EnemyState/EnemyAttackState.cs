using UnityEngine;

public class EnemyAttackState : StateBase
{
    private AttackData _attack;
    public EnemyAttackState(EnemyAI enemyAI, AttackData attack) : base(enemyAI)
    {
        _attack = attack;
    }
    protected override string GetAnimationName() => _attack.animationTrigger;

    public override void Enter()
    {
        base.Enter();
        // Animation Event will handle OnAttackHit
    }
}