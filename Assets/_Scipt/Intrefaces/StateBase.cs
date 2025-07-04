using UnityEngine;

public abstract class StateBase : IState
{
    protected MonoBehaviour _controller;
    protected float _timer;
    protected float _duration;

    protected StateBase(MonoBehaviour controller)
    {
        _controller = controller;
    }

    public virtual void Enter()
    {
        _timer = 0f;
        _duration = GetAnimationDuration(GetAnimationName());
        if (_controller is PlayerController player)
            player.TriggerAnimation(GetAnimationName());
        else if (_controller is EnemyAI enemy)
            enemy.GetAnimator().SetTrigger(GetAnimationName());
        Debug.Log($"Entering {GetType().Name}");
    }

    public virtual void Execute()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            if (_controller is PlayerController player)
                StateManager.Instance.ChangeState(new IdleState(player));
            else if (_controller is EnemyAI enemy)
                enemy.ChangeEnemyState(new EnemyIdleState(enemy));
        }
    }

    public virtual void Exit()
    {
        Debug.Log($"Exiting {GetType().Name}");
    }

    protected abstract string GetAnimationName();

    protected float GetAnimationDuration(string clipName)
    {
        Animator animator = _controller is PlayerController player ? player.Animator : (_controller as EnemyAI).GetAnimator();
        if (animator == null)
        {
            Debug.LogWarning("Animator is null in GetAnimationDuration!");
            return 0.5f;
        }
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        foreach (var clip in controller.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
            Debug.LogWarning($"Animation {clipName} not found in {animator.name}!");
        return 0.5f;
    }
}