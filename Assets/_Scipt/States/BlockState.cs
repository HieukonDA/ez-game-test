using UnityEngine;

public class BlockState : IState
{
    PlayerController _playerController;
    public BlockState(PlayerController playerController)
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