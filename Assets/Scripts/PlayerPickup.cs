using UnityEngine;
using System.Collections.Generic;

public class PlayerPickup : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public PickupTextManager pickupText;
    public HashSet<Collider> processedItems = new HashSet<Collider>();

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        pickupText = GetComponent<PickupTextManager>();
        
        if (inventoryManager == null)
            Debug.LogError("[Pickup] No InventoryManager found on player!");
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if we've already processed this item
        if (processedItems.Contains(other))
        {
            Debug.Log($"[Pickup] Item already processed, skipping: {other.name}");
            return;
        }

        // Use tag instead of checking component directly
        if (other.CompareTag("Pickup") || other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                Debug.Log($"[Pickup] Found item: {item.itemName} (Stackable: {item.isStackable}, Usable: {item.isUsable})");
                
                // Add to processed items before trying to add to inventory
                processedItems.Add(other);

                if (inventoryManager.AddItem(item))
                {
                    Debug.Log($"[Pickup] Successfully added {item.itemName} to inventory");
                    if (pickupText != null)
                    {
                        pickupText.ShowPickupText($"Picked up {item.itemName}");
                    }
                    Destroy(other.gameObject);
                }
                else
                {
                    // If we couldn't add the item, remove it from processed items
                    processedItems.Remove(other);
                    string message = item.isStackable ? "Inventory Full!" : "You don't have available option in your inventory. Use any to make it available.";
                    if (pickupText != null)
                    {
                        pickupText.ShowPickupText(message);
                    }
                    Debug.Log($"[Pickup] Could not add {item.itemName} - {message}");
                }
            }
        }
    }

    void OnDisable()
    {
        processedItems.Clear();
    }
}
