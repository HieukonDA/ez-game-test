using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Combat/AttackData")]
public class AttackData : ScriptableObject
{
    public string actionName;
    public string animationTrigger;
    public int damage;
    public float staminaCost;
}