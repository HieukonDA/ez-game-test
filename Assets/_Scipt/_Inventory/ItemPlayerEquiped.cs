using UnityEngine;
using UnityEngine.UI;

public class ItemPlayerEquiped : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    public ItemData ItemData => _itemData;
    public int ItemID => _itemData.itemID;

    private Image _equippedImage;

    private void Awake()
    {
        _equippedImage = GetComponent<Image>();
    }

    public void Setup(ItemData item)
    {
        if (item == null) return;
        _itemData = item;
        _equippedImage.sprite = item.icon;
    }
}