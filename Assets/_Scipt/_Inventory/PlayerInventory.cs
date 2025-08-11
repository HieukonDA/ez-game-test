using System.Collections.Generic;
using Unity.Services.CloudSave.Models;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // [Header("All Items")]
    // public List<ItemData> ownedItems = new List<ItemData>();

    // [Header("Equipped Items")]
    // public List<ItemData> equippedItems = new List<ItemData>();

    // public bool OwnsItem(ItemData item)
    // {
    //     return ownedItems.Contains(item);
    // }

    // public bool IsEquipItem(ItemData item)
    // {
    //     return equippedItems.Contains(item);
    // }

    // public bool CanEquipItem(ItemData item)
    // {
    //     const int MAX_EQUIPPED_ITEMS = 4;
    //     return OwnsItem(item) && !IsEquipItem(item) && equippedItems.Count < MAX_EQUIPPED_ITEMS;
    // }

    // public bool EquipItem(ItemData item)
    // {
    //     if (CanEquipItem(item))
    //     {
    //         equippedItems.Add(item);
    //         return true;
    //     }
    //     return false;
    // }

    // public void UnEquipItem(ItemData item)
    // {
    //     if (equippedItems.Contains(item))
    //     {
    //         equippedItems.Remove(item);
    //     }
    // }

    // public void AddItem(ItemData item)
    // {
    //     if (!ownedItems.Contains(item))
    //     {
    //         ownedItems.Add(item);
    //     }
    // }
}