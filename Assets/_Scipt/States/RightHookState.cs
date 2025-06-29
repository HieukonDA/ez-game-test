using UnityEngine;

public class RightHookState : IState
{
    PlayerController _playerController;
    public RightHookState(PlayerController playerController)
    {
        _playerController = playerController;
    }
    public void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    public void Execute()
    {
        _playerController.Idle();
    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}