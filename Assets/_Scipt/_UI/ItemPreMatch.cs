using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Authentication;
using System.Collections.Generic;
using Unity.Services.CloudSave.Models;

public class ItemPreMatch : Panel
{
    [SerializeField] private Button _fightButton;
    [SerializeField] private Button _backButton;
    public List<ItemPlayerEquiped> itemEquipped = new List<ItemPlayerEquiped>();
    [SerializeField] private GameObject _itemEquippedPrefab;
    [SerializeField] private Transform _itemSkillPlayerContainer;
    private bool isOpenedPanle = false;
    public System.Action<ItemData> _OnClickItemEquipped;


    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        _fightButton.onClick.AddListener(OnFightButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
        foreach (var item in itemEquipped)
        {
            if (item == null || item.ItemData == null)
            {
                Debug.LogError("ItemPlayerEquiped or ItemData is null in ItemPreMatch.");
                continue;
            }
            else
            {
                item.GetComponent<Button>().onClick.AddListener(() => OnOpenInventoryClicked(item.ItemData));
            }

        }

        LoadData();
        base.Initialize();
    }

    public async void LoadData()
    {
        var equippedItems = await MenuManager.Singleton.LoadItemEquippedAsync();
        LoadItemEquippedAsync(equippedItems);
    }

    private void OnFightButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Close("itemprematch");
        SceneManager.LoadScene("Match");
        InventoryManager.Instance.SaveItemEquippedAsync();
        Time.timeScale = 1;
    }

    private void OnBackButtonClicked()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
        PanelManager.Close("itemprematch");
        SceneManager.LoadScene("MainMenu");
        InventoryManager.Instance.SaveItemEquippedAsync();
        Time.timeScale = 1;
    }

    private void OnOpenInventoryClicked(ItemData itemData)
    {
        Debug.LogWarning("OnOpenInventoryClicked called with item: " + itemData.itemName);
        isOpenedPanle = !isOpenedPanle;
        if (isOpenedPanle)
        {
            PanelManager.Open("inventory");
            _OnClickItemEquipped?.Invoke(itemData);
        }
        else
        {
            PanelManager.Close("inventory");
        }
        AudioManager.Instance.PlaySound("ButtonClick");
    }

    public void AddItemEquipped(ItemData item)
    {
        if (item == null)
        {
            Debug.LogError("ItemPlayerEquiped or ItemData is null in AddItemEquipped.");
            return;
        }
        ItemPlayerEquiped itemEquipped = Instantiate(_itemEquippedPrefab, _itemSkillPlayerContainer).GetComponent<ItemPlayerEquiped>();
        itemEquipped.Setup(item);
        this.itemEquipped.Add(itemEquipped);
        itemEquipped.GetComponent<Button>().onClick.AddListener(() => OnOpenInventoryClicked(item));

    }

    public void RemoveItemEquipped(ItemData item)
    {
        if (item == null)
        {
            Debug.LogError("ItemPlayerEquiped or ItemData is null in RemoveItemEquipped.");
            return;
        }

        ItemPlayerEquiped itemRemove = itemEquipped.Find(i => i.ItemData != null && i.ItemData.itemID == item.itemID);
        if (itemRemove == null)
        {
            Debug.LogError($"ItemPlayerEquiped with itemID {item.itemID} not found in RemoveItemEquipped.");
            return;
        }
        itemEquipped.Remove(itemRemove);
        Destroy(itemRemove.gameObject);
    }

    public void LoadItemEquippedAsync(List<ItemData> itemDataList)
    {
        if (itemDataList == null || itemDataList.Count == 0)
        {
            Debug.LogWarning("No items to load for ItemPreMatch.");
            return;
        }

        for (int i = _itemSkillPlayerContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_itemSkillPlayerContainer.GetChild(i).gameObject);
        }
        itemEquipped.Clear();

        foreach (var itemData in itemDataList)
        {
            if (itemData == null)
            {
                Debug.LogError("ItemData is null in LoadItemEquippedAsync.");
                continue;
            }
            AddItemEquipped(itemData);
        }
    }

    public bool IsEquippedItem(ItemData item)
    {
        foreach (var equippedItem in itemEquipped)
        {
            if (equippedItem.ItemData != null && equippedItem.ItemData.itemID == item.itemID)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanEquipItem(ItemData item)
    {
        const int MAX_EQUIPPED_ITEMS = 4;
        return itemEquipped.Count < MAX_EQUIPPED_ITEMS && !IsEquippedItem(item);
    }

    public List<ItemData> GetItemEquipped()
    {
        List<ItemData> equippedItems = new List<ItemData>();
        foreach (var item in itemEquipped)
        {
            equippedItems.Add(item.ItemData);
        }
        return equippedItems;
    }

}