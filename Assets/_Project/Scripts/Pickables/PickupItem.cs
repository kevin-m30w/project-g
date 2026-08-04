using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
   [SerializeField] private ItemData itemData;

    public ItemData Data => itemData;

    public string GetInteractPrompt()
    {
        string sizeTag = itemData.itemSize == ItemSize.Large ? " (Two-Handed)" : "";
        return $"Pick up {itemData.itemName}{sizeTag} [E]";
    }

    public void Interact(PlayerController player)
    {
        Debug.Log("Interact method called on item!");

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            Debug.Log("Found PlayerInventory, trying pickup...");
            bool success = inventory.TryPickupItem(this);
            Debug.Log($"Pickup result: {success}");
        }
        else
        {
            Debug.LogError("PlayerInventory component NOT FOUND on player!");
        }
    }
}
