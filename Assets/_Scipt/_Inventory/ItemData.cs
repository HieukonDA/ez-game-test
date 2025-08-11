using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int itemID;
    public float coolDownTime;
    public float bonusDamagePercent;
    public string description;

    [Header("Item model")]
    public Mesh itemMesh;
    public Material itemMaterial;


}