using UnityEngine;

[CreateAssetMenu(fileName = "ComboData", menuName = "Combat/ComboData")]
public class ComboData : ScriptableObject
{
    public string[] sequence;
    public int damageBonus;
}