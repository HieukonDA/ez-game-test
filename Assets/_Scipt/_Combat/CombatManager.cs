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
        ActionType enemyAction = _enemyAI.GenerateAction();

        (ActionType playerResult, ActionType enemyResult) = CombatResolve.Resolve(playerAction, enemyAction);

        //take damage
        int playerDamage = IsPunch(playerResult) ? 10 : 0;
        int enemyDamage = IsPunch(enemyResult) ? 10 : 0;

        if (IsHit(playerResult)) _playerController.ReceiveHit(playerResult, enemyDamage);
        if (IsHit(enemyResult)) _enemyAI.ReceiveHit(enemyResult, playerDamage);

        //perform action
        _playerController.PerformAction(playerResult);
        _enemyAI.PerformAction(enemyResult);

        if (playerResult == ActionType.HeadHit) _enemyAI.KnockOut();
        if (enemyResult == ActionType.HeadHit) _playerController.KnockOut();
    }

    private bool IsHit(ActionType action)
    {
        return action == ActionType.HeadHit || action == ActionType.StomachHit || action == ActionType.KidneyHit;
    }
    
    private bool IsPunch(ActionType action)
    {
        return action == ActionType.HeadPunch || 
               action == ActionType.KidneyPunchLeft || 
               action == ActionType.KidneyPunchRight;
    }

    
}
