using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Combat/AttackData")]
public class AttackData : ScriptableObject
{
    public string actionName;
    public string animationTrigger;
    public int damage;
    public float staminaCost;
    public float bonusDamagePercent = 0;

    public int FinalDamage()
    {
        return Mathf.RoundToInt(damage * (1 + bonusDamagePercent / 100f));
    }
}