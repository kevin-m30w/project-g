using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int hotbarSize = 4;
    [SerializeField] private float dropForwardForce = 2f;

    [Header("Current State")]
    private ItemData[] _hotbarSlots;
    private int _activeSlotIndex = 0;

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform itemHoldSocket;
    [SerializeField] private Transform dropPoint;


    private ItemData _carriedLargeItem = null;
    private GameObject _currentHeldInstance = null;

    public bool IsCarryingLargeItem => _carriedLargeItem != null;



    private void Awake()
    {
        _hotbarSlots = new ItemData[hotbarSize];
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.DropEvent += HandleDrop;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.DropEvent -= HandleDrop;
        }
    }

    public bool TryPickupItem(PickupItem worldItem)
    {
        ItemData data = worldItem.Data;

        // Case 1: Large Item
        if (data.itemSize == ItemSize.Large)
        {
            if (IsCarryingLargeItem)
            {
                Debug.Log("Hands are full carrying a large item!");
                return false;
            }

            _carriedLargeItem = data;
            Destroy(worldItem.gameObject); // Remove world object
            UpdateHeldItemVisuals();
            return true;
        }

        // Case 2: Small Item
        if (IsCarryingLargeItem)
        {
            Debug.Log("Can't pick up small items while carrying a large item!");
            return false;
        }

        // Try adding to empty slot
        for (int i = 0; i < hotbarSize; i++)
        {
            if (_hotbarSlots[i] == null)
            {
                _hotbarSlots[i] = data;
                Destroy(worldItem.gameObject);

                // Automatically switch to picked up item if in current slot
                if (i == _activeSlotIndex)
                {
                    UpdateHeldItemVisuals();
                }

                Debug.Log($"Picked up {data.itemName} into Hotbar Slot {i + 1}");
                return true;
            }
        }

        Debug.Log("Hotbar is full!");
        return false;

    }


    private void UpdateHeldItemVisuals()
    {
        if (_currentHeldInstance != null)
        {
            Destroy(_currentHeldInstance);
        }

        ItemData dataToDisplay = null;
        if (IsCarryingLargeItem)
        {
            dataToDisplay = _carriedLargeItem;
        }
        else if (_hotbarSlots[_activeSlotIndex] != null)
        {
            dataToDisplay = _hotbarSlots[_activeSlotIndex];
        }

        // Instantiate held visual object if available
        if (dataToDisplay != null && dataToDisplay.heldPrefab != null)
        {
            _currentHeldInstance = Instantiate(dataToDisplay.heldPrefab, itemHoldSocket);
            _currentHeldInstance.transform.localPosition = Vector3.zero;
            _currentHeldInstance.transform.localRotation = Quaternion.identity;
        }
    }

    private void HandleDrop()
    {
        DropCurrentItem();
    }

    private void DropCurrentItem()
    {
        ItemData itemToDrop = null;

        // 1. Check if holding a large item first
        if (IsCarryingLargeItem)
        {
            itemToDrop = _carriedLargeItem;
            _carriedLargeItem = null;
        }
        // 2. Otherwise drop current hotbar item
        else if (_hotbarSlots[_activeSlotIndex] != null)
        {
            itemToDrop = _hotbarSlots[_activeSlotIndex];
            _hotbarSlots[_activeSlotIndex] = null;
        }

        // If we found an item to drop
        if (itemToDrop != null && itemToDrop.worldPrefab != null)
        {
            // Destroy visual model in hand
            if (_currentHeldInstance != null)
            {
                Destroy(_currentHeldInstance);
            }

            // Spawn physical world prefab
            Vector3 spawnPos = dropPoint != null ? dropPoint.position : transform.position + transform.forward;
            Quaternion spawnRot = dropPoint != null ? dropPoint.rotation : transform.rotation;

            GameObject droppedObject = Instantiate(itemToDrop.worldPrefab, spawnPos, spawnRot);

            // Give it a tiny toss forward
            if (droppedObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Vector3 throwDir = dropPoint != null ? dropPoint.forward : transform.forward;
                rb.AddForce(throwDir * dropForwardForce, ForceMode.VelocityChange);
            }

            Debug.Log($"Dropped {itemToDrop.itemName}");
        }
    }
}
