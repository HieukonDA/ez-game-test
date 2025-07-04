using UnityEngine;

public class StateFactory
{
    public static IState CreateState(string action, PlayerController player)
    {
        return action switch
        {
            "LeftJab" => new LeftJabState(player),
            "RightJab" => new RightJabState(player),
            "LeftHook" => new LeftHookState(player),
            "RightHook" => new RightHookState(player),
            "LeftUpperCut" => new LeftUpperCutState(player),
            "RightUpperCut" => new RightUpperCutState(player),
            "Dodge" => new DodgeState(player),
            "Block" => new BlockState(player),
            _ => new IdleState(player)
        };
    }
}