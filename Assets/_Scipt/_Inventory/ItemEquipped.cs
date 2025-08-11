using UnityEngine;

public class ItemEquipped : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    public void EquipItem(ItemData itemData)
    {
        _meshFilter.mesh = itemData.itemMesh;
        _meshRenderer.material = itemData.itemMaterial;
    }

    public void ClearItem()
    {
        _meshFilter.mesh = null;
        _meshRenderer.material = null;
    }
}