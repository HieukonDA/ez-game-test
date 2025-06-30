using UnityEngine;

public class RightJabState : IState
{
    PlayerController _playerController;
    private float _timer;
    private float _duration;

    public RightJabState(PlayerController playerController)
    {
        _playerController = playerController;
    }
    public void Enter()
    {
        _playerController.RightJab();
        _duration = GetAnimationDuration("RightJab");
        _timer = 0f;
        Debug.Log("Entering Idle State");
    }

    public void Execute()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            StateManager.Instance.ChangeState(new IdleState(_playerController));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }

    private float GetAnimationDuration(string clipName)
    {
        RuntimeAnimatorController controller = _playerController.Animator.runtimeAnimatorController;

        foreach (var clip in controller.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"Animation {clipName} not found!");
        return 0.5f; // fallback
    }

}