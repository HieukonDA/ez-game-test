using UnityEngine;

public class RightJabState : IState
{
    PlayerController _playerController;
    public RightJabState(PlayerController playerController)
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