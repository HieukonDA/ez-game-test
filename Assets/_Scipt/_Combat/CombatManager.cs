using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyAI _enemyAI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SubmitAction(ActionType playerAction)
    {
        AudioManager.Instance.PlaySound("Punch");
        ActionType enemyAction = _enemyAI.GenerateAction();
        (ActionType playerResult, ActionType enemyResult) = playerAction == ActionType.None 
            ? (ActionType.None, enemyAction) 
            : CombatResolve.Resolve(playerAction, enemyAction);

        //take damage
        int playerDamage = IsPunch(playerResult);
        int enemyDamage = IsPunch(enemyResult);

        if (IsHit(playerResult))
        {
            _playerController.ReceiveHit(playerResult, playerDamage);
        }
        if (IsHit(enemyResult))
        {
            _enemyAI.ReceiveHit(enemyResult, enemyDamage);
        }

        // if (playerAction != ActionType.None)
        // {
        //     _playerController.PerformAction(playerResult);
        // }
        _enemyAI.PerformAction(enemyResult);

        if (_playerController.CurrentHealth <= 0)
        {
            _playerController.KnockOut();
            EndMatch(false);
        }
        if (_enemyAI.CurrentHealth <= 0)
        {
            _enemyAI.KnockOut();
            EndMatch(true);
        }
    }

    private bool IsHit(ActionType action)
    {
        return action == ActionType.HeadHit || action == ActionType.KidneyHit || action == ActionType.StomachHit;
    }

    private int IsPunch(ActionType action)
    {
        return action switch
        {
            ActionType.HeadHit => 10,
            ActionType.KidneyHit => 5,
            ActionType.StomachHit => 7,
            _ => 0
        };
    }
    
    private void EndMatch(bool playerWon)
    {
        // Stop further actions
        Time.timeScale = 0; // Pause game (replace with proper game over UI later)
        Debug.Log(playerWon ? "Player Wins!" : "Enemy Wins!");
    }

    
}
