using UnityEngine;

public abstract class StateBase : IState
{
    protected PlayerController _playerController;
    protected float _timer;
    protected float _duration;

    protected StateBase(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public virtual void Enter()
    {
        _timer = 0f;
        _duration = GetAnimationDuration(GetAnimationName());
        _playerController.TriggerAnimation(GetAnimationName());
        Debug.Log($"Entering {GetType().Name}");
    }

    public virtual void Execute()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            StateManager.Instance.ChangeState(new IdleState(_playerController));
        }
    }

    public virtual void Exit()
    {
        Debug.Log($"Exiting {GetType().Name}");
    }

    protected abstract string GetAnimationName();

    protected float GetAnimationDuration(string clipName)
    {
        RuntimeAnimatorController controller = _playerController.Animator.runtimeAnimatorController;
        foreach (var clip in controller.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        Debug.LogWarning($"Animation {clipName} not found!");
        return 0.5f; // Fallback
    }
}