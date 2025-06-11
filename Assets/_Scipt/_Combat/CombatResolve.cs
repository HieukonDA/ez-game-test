using UnityEngine;
using UnityEngine.EventSystems;

public class CombatResolve : MonoBehaviour
{
    public static CombatResolve Instance { get; private set; }
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


    [SerializeField] private Transform _Player;
    [SerializeField] private Transform _Enemy;
    public static (ActionType playerResult, ActionType enemyResult) Resolve(ActionType playerAction, ActionType enemyAction)
    {
        // Quy tắc: nếu cả hai cùng punch → cả hai bị hit
        if (IsPunch(playerAction) && IsPunch(enemyAction))
        {
            return (GetHitFrom(playerAction), GetHitFrom(enemyAction));
        }

        // Nếu player KO và enemy punch → enemy bị KO
        if (IsKnockOut(playerAction) && IsPunch(enemyAction))
        {
            return (playerAction, GetHitFrom(playerAction)); // enemy trúng
        }

        // Nếu enemy KO và player punch → player bị KO
        if (IsKnockOut(enemyAction) && IsPunch(playerAction))
        {
            return (GetHitFrom(enemyAction), enemyAction); // player trúng
        }

        // Nếu chỉ 1 bên Punch → bên kia dính hit
        if (IsPunch(playerAction) && !IsPunch(enemyAction))
        {
            return (playerAction, GetHitFrom(playerAction));
        }

        if (IsPunch(enemyAction) && !IsPunch(playerAction))
        {
            return (GetHitFrom(enemyAction), enemyAction);
        }

        return (playerAction, enemyAction);
    }

    private static bool IsPunch(ActionType action)
    {
        return action == ActionType.HeadPunch ||
               action == ActionType.KidneyPunchLeft ||
               action == ActionType.KidneyPunchRight ||
               action == ActionType.StomachPunch;
    }

    private static bool IsKnockOut(ActionType action)
    {
        return action == ActionType.HeadHit;
    }

    private static ActionType GetHitFrom(ActionType punch)
    {
        return punch switch
        {
            ActionType.HeadPunch => ActionType.HeadHit,
            ActionType.StomachPunch => ActionType.StomachHit,
            ActionType.KidneyPunchLeft => ActionType.KidneyHit,
            ActionType.KidneyPunchRight => ActionType.KidneyHit,
            ActionType.HeadHit => ActionType.HeadHit,
            _ => punch // fallback
        };
    }
}