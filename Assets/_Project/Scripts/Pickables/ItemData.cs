using UnityEngine;

public enum ItemSize
{
    Small, 
    Large
}

[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName = "NewItem";
    public Sprite itemIcon;
    public ItemSize itemSize = ItemSize.Small;
    public int scrapValue = 10;
    public float weightKg = 5f;

    [Header("Prefabs")]
    public GameObject worldPrefab;
    public GameObject heldPrefab;
}
