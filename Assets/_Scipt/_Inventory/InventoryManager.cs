using System;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    [SerializeField] private ItemPreMatch _itemPreMatch;
    [SerializeField] private InventoryPrematch _inventoryPrematch;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _itemPreMatch = FindObjectOfType<ItemPreMatch>();
        _inventoryPrematch = FindObjectOfType<InventoryPrematch>();
    }

    public async void SaveItemEquippedAsync()
    {
        if (!MenuManager.Singleton.IsSignedIn())
        {
            Debug.LogError("not signed in, Saving item equipped to cloud...");
            return;
        }

        try
        {
            await MenuManager.Singleton.SaveItemEquippedAsync(_itemPreMatch.GetItemEquipped());
            Debug.Log("Item equipped saved to cloud successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save item equipped to cloud. {" + e.Message + "}");
            throw;
        }


    }

    public List<ItemData> GetItemDataFromItemIds(List<int> itemIds)
    {
        List<ItemData> itemDatas = new List<ItemData>();
        foreach (var itemId in itemIds)
        {
            ItemData itemData = _inventoryPrematch.itemSlots.Find(slot => slot.ItemData != null && slot.ItemID == itemId)?.ItemData;
            if (itemData != null)
            {
                itemDatas.Add(itemData);
            }
            else
            {
                Debug.LogWarning($"Item with ID {itemId} not found in inventory.");
            }
        }
        return itemDatas;
    }

    // private async void SaveInventoryToCloud()
    // {
    //     if (MenuManager.Singleton.IsSignedIn())
    //     {
    //         Debug.LogError("not signed in, Saving inventory to cloud...");
    //         return;
    //     }

    //     try
    //     {
    //         await MenuManager.Singleton.SaveInventoryAsync(_playerInventory);
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogError("Failed to save inventory to cloud. {" + e.Message + "}");
    //         throw;
    //     }
    // }

    // [Header("Inventory Settings")]
    // [SerializeField] private PlayerInventory _playerInventory;

    // [Header("test item")]
    // [SerializeField] private List<ItemData> _testItems = new List<ItemData>();
    // [SerializeField] private bool addTestItemsOnStart = false;
    // // event to notify when an item is added
    // public static event System.Action<PlayerInventory> OnInventoryChanged;



    // private void Start()
    // {
    //     if (addTestItemsOnStart)
    //     {
    //         foreach (var item in _testItems)
    //         {
    //             AddItem(item);
    //         }
    //     }

    //     LoadInventoryFromCloud();
    // }

    // private void AddItem(ItemData item)
    // {
    //     if (item != null)
    //     {
    //         _playerInventory.AddItem(item);
    //         OnInventoryChanged?.Invoke(_playerInventory);
    //         SaveInventoryToCloud();
    //     }
    // }

    // public bool EquipItem(ItemData item)
    // {
    //     bool success = _playerInventory.EquipItem(item);
    //     if (success)
    //     {
    //         OnInventoryChanged?.Invoke(_playerInventory);
    //         SaveInventoryToCloud();
    //     }
    //     return success;
    // }

    // public void UnEquipItem(ItemData item)
    // {
    //     _playerInventory.UnEquipItem(item);
    //     OnInventoryChanged?.Invoke(_playerInventory);
    //     SaveInventoryToCloud();
    // }

    // public PlayerInventory GetInventory()
    // {
    //     return _playerInventory;
    // }

    // public List<ItemData> GetEquippedItems()
    // {
    //     return new List<ItemData>(_playerInventory.equippedItems);
    // }

    // public List<ItemData> GetOwnedItems()
    // {
    //     return new List<ItemData>(_playerInventory.ownedItems);
    // }

    // #region Cloud Save Integration

    // private async void LoadInventoryFromCloud()
    // {
    //     if (!MenuManager.Singleton.IsSignedIn())  // ✓ Don't load when NOT signed in
    //     {
    //         Debug.Log("Not signed in, using local inventory");
    //         return;
    //     }

    //     try
    //     {
    //         var cloudInventory = await MenuManager.Singleton.LoadInventoryAsync();  // ✓ Load from cloud
    //         if (cloudInventory != null)
    //         {
    //             _playerInventory = cloudInventory;
    //             OnInventoryChanged?.Invoke(_playerInventory);
    //         }
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogError($"Failed to load inventory: {e.Message}");
    //     }
    // }

    // 

    // #endregion
}