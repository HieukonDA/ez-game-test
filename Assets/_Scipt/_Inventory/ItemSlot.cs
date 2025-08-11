using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;

    [Header("UI References")]
    public Sprite itemIcon;
    public Button selectButton;
    public GameObject equippedIndicator;
    public GameObject selectedPanel;

    private InventoryPrematch parentUI;

    public int ItemID => _itemData.itemID;
    public ItemData ItemData => _itemData;
    public System.Action<ItemData> OnItemSelected;

    private void Start()
    {
        selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

    private void OnSelectButtonClicked()
    {
        OnItemSelected?.Invoke(_itemData);
    }


    // public void Setup(ItemData item, InventoryPrematch parent)
    // {
    //     _itemData = item;
    //     parentUI = parent;

    //     if (item != null)
    //     {
    //         // Set visuals
    //         itemIcon = item.icon;

    //         // Set equipped status
    //         bool isEquipped = InventoryManager.Instance.GetInventory().IsEquipItem(item);
    //         equippedIndicator.SetActive(isEquipped);

    //         // Setup button
    //         selectButton.onClick.RemoveAllListeners();
    //         selectButton.onClick.AddListener(() => OnSlotClicked());
    //     }
    // }
    
    // private void OnSlotClicked()
    // {
    //     if (_itemData != null && parentUI != null)
    //     {
    //         parentUI.OnItemSelected(_itemData);
    //     }
    // }
}