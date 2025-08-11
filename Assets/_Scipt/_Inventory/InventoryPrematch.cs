using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class InventoryPrematch : Panel
{
    [Header("Item details")]
    [SerializeField] private GameObject _itemDetailsPanel;
    [SerializeField] private Button _equipButton;
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemStatsText;

    [Header("Item Prematch")]
    [SerializeField] private ItemPreMatch _itemPreMatch;

    [Header("UI Elements")]
    [SerializeField] private Transform _itemSlotContainer;
    [SerializeField] private ItemSlot _itemSlotPrefab;

    [Header("Item Slots")]
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    private ItemData _selectedItem = null;


    private void Start()
    {
        _itemPreMatch._OnClickItemEquipped += OnItemClicked;
        foreach (var itemslot in itemSlots)
        {
            if (itemslot != null)
            {
                itemslot.OnItemSelected += OnItemClicked;
            }
        }
        
        
    }

    private void ButtonEquipClicked()
    {
        _equipButton.onClick.RemoveAllListeners();

        if (_itemPreMatch.IsEquippedItem(_selectedItem))
        {
            _equipButton.onClick.AddListener(UnEquipSelectedItem);
        }
        else
        {
            _equipButton.onClick.AddListener(EquipSelectedItem);
        }
    }

    private void OnItemClicked(ItemData item)
    {
        if (item == null) return;

        _selectedItem = item;

        ButtonEquipClicked();
        UpdateItemDetails(item);
        HighLightSelectedItem(item);    
    }

    private void UpdateItemDetails(ItemData item)
    {
        _itemImage.sprite = item.icon;
        _itemStatsText.text = $"Item: {item.itemName}\n" +
                              $"Damage: {item.bonusDamagePercent}\n" +
                              $"Cooldown: {item.coolDownTime} seconds";

        bool isEquipped = _itemPreMatch.IsEquippedItem(item);
        bool canEquip = _itemPreMatch.CanEquipItem(item);

        _equipButton.GetComponentInChildren<TextMeshProUGUI>().text =
            isEquipped ? "UNEQUIP" : "EQUIP";
        _equipButton.interactable = isEquipped || canEquip;
    }

    private void HighLightSelectedItem(ItemData item)
    {
        Debug.LogError($"Highlighting item: {item.itemName}");
        foreach (var itemSlot in itemSlots)
        {
            if (itemSlot != null && itemSlot.ItemID == item.itemID)
            {
                itemSlot.selectedPanel.SetActive(false);
            }
            else
            {
                itemSlot.selectedPanel.SetActive(true);
            }
        }
    }

    private void EquipSelectedItem()
    {
        Debug.LogError($"Equip Selected item: {_selectedItem?.itemName}");
        if (_selectedItem == null) return;

        if (_itemPreMatch.IsEquippedItem(_selectedItem)) return;
        if (!_itemPreMatch.CanEquipItem(_selectedItem)) return;
        

        _itemPreMatch.AddItemEquipped(_selectedItem);

        UpdateItemDetails(_selectedItem);
        ButtonEquipClicked();
        
        InventoryManager.Instance.SaveItemEquippedAsync();
        AudioManager.Instance.PlaySound("ButtonClick");
    }

    private void UnEquipSelectedItem()
    {
        Debug.LogError($"Unequip Selected item: {_selectedItem?.itemName}");
        if (_selectedItem == null) return;

        if (!_itemPreMatch.IsEquippedItem(_selectedItem)) return;

        _itemPreMatch.RemoveItemEquipped(_selectedItem);
        
        UpdateItemDetails(_selectedItem);
        ButtonEquipClicked();
        InventoryManager.Instance.SaveItemEquippedAsync();
        
        AudioManager.Instance.PlaySound("ButtonClick");
    }








    

    [Header("Item Data")]
    private List<GameObject> _itemSlot = new List<GameObject>();
    

    // public void Start()
    // {
    //     Initialization();

    //     InventoryManager.OnInventoryChanged += OnInventoryChanged;

    //     RefreshInventoryUI();
    // }

    // private void OnDestroy()
    // {
    //     InventoryManager.OnInventoryChanged -= OnInventoryChanged;
    // }

    // private void Initialization()
    // {
    //     _equipButton.onClick.AddListener(EquipSelectItem);

    //     if (_itemDetailsPanel != null)
    //     {
    //         _itemDetailsPanel.SetActive(false);
    //     }
    // }

    // private void OnInventoryChanged(PlayerInventory playerInventory)
    // {
    //     RefreshInventoryUI();

    //     if (_selectedItem != null)
    //     {
    //         UpdateItemDetail(_selectedItem);
    //     }
    // }

    // private void RefreshInventoryUI()
    // {
    //     foreach (var slot in _itemSlots)
    //     {
    //         if (slot != null) Destroy(slot);
    //     }
    //     _itemSlots.Clear();

    //     var ownedItems = InventoryManager.Instance.GetOwnedItems();
    //     foreach (var item in ownedItems)
    //     {
    //         CreateItemSlot(item);
    //     }
    // }

    // private void CreateItemSlot(ItemData item)
    // {
    //     GameObject slotObj = Instantiate(_itemSlotPrefab.gameObject, _itemSlotContainer);


    //     ItemSlot slot = slotObj.GetComponent<ItemSlot>();
    //     slot.Setup(item, this);

    //     _itemSlot.Add(slotObj);
    // }

    // public void OnItemSelected(ItemData item)
    // {
    //     _selectedItem = item;
    //     UpdateItemDetail(item);

    // }

    // private void UpdateItemDetail(ItemData item)
    // {
    //     _itemImage.sprite = item.icon;
    //     _itemStatsText.text = $"Item: {item.itemName}\n" +
    //                           $"Damage: {item.damageEffect}\n" +
    //                           $"Cooldown: {item.coolDownTime} seconds";

    //     bool isEquipped = InventoryManager.Instance.GetInventory().IsEquipItem(item);
    //     bool canEquip = InventoryManager.Instance.GetInventory().CanEquipItem(item);

    //     _equipButton.GetComponentInChildren<TextMeshProUGUI>().text =
    //         isEquipped ? "UNEQUIP" : "EQUIP";
    //     _equipButton.interactable = isEquipped || canEquip;
    // }

    // private void EquipSelectItem()
    // {
    //     if (_selectedItem == null) return;

    //     bool isEquipped = InventoryManager.Instance.GetInventory().IsEquipItem(_selectedItem);

    //     if (isEquipped)
    //     {
    //         // Unequip
    //         InventoryManager.Instance.UnEquipItem(_selectedItem);
    //         Debug.Log($"Unequipped: {_selectedItem.itemName}");
    //     }
    //     else
    //     {
    //         // Equip
    //         bool success = InventoryManager.Instance.EquipItem(_selectedItem);
    //         if (success)
    //         {
    //             Debug.Log($"Equipped: {_selectedItem.itemName}");
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"Cannot equip {_selectedItem.itemName} - slots full or not owned");
    //         }
    //     }

    //     // Close details panel after action
    //     _itemDetailsPanel.SetActive(false);
    //     _selectedItem = null;
    // }
}