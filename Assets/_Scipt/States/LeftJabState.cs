using UnityEngine;

public class LeftJabState : IState
{
    PlayerController _playerController;
    public LeftJabState(PlayerController playerController)
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