using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] inventorySlots = new GameObject[3];
    public Image[] slotImages = new Image[3];
    public TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[3];
    
    public ItemData[] inventory;
    public int[] stackCounts;

    void Awake()
    {
        // Initialize arrays
        inventory = new ItemData[3];
        stackCounts = new int[3];

        Debug.Log("[Inventory] Initializing inventory with " + inventory.Length + " slots");

        // Initialize UI
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = null;
            stackCounts[i] = 0;
            
            if (slotImages != null && slotImages[i] != null)
            {
                slotImages[i].color = new Color(1, 1, 1, 0.5f);
                if (slotTexts != null && slotTexts[i] != null)
                {
                    slotTexts[i].text = "0";  // Initialize with 0
                }
            }
        }
    }

    public bool AddItem(Item item)
    {
        Debug.Log($"[Inventory] Attempting to add: {item.itemName}");

        // For stackable items (like Health Boost)
        if (item.isStackable)
        {
            // First try to find existing stack
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] != null)
                {
                    Debug.Log($"[Inventory] Checking slot {i}: Found {inventory[i].itemName}");
                    
                    // Compare item types
                    if (inventory[i].itemType == item.GetType())
                    {
                        stackCounts[i]++;
                        Debug.Log($"[Inventory] Stacked {item.itemName} in slot {i}. New count: {stackCounts[i]}");
                        UpdateSlotUI(i);
                        return true;
                    }
                }
            }
        }

        // If we get here, either:
        // 1. Item is not stackable
        // 2. No existing stack was found
        // So find first empty slot
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                // Create ItemData from the item
                inventory[i] = new ItemData(item);
                stackCounts[i] = 1;
                Debug.Log($"[Inventory] Added {item.itemName} to slot {i}");
                UpdateSlotUI(i);
                return true;
            }
        }

        Debug.Log("[Inventory] No empty slots available");
        return false;
    }

    private void UpdateSlotUI(int slot)
    {
        if (slot < 0 || slot >= inventory.Length) return;

        if (slotImages != null && slotImages[slot] != null)
        {
            if (inventory[slot] != null)
            {
                // Slot has an item
                slotImages[slot].sprite = inventory[slot].itemSprite;
                slotImages[slot].color = Color.white;

                if (slotTexts != null && slotTexts[slot] != null)
                {
                    if (inventory[slot].isStackable)
                    {
                        // For stackable items, show the stack count
                        slotTexts[slot].text = stackCounts[slot].ToString();
                        Debug.Log($"[Inventory] Updated UI for slot {slot} with stack count: {stackCounts[slot]}");
                    }
                    else
                    {
                        // For non-stackable items, show "1" instead of slot number
                        slotTexts[slot].text = "1";
                        Debug.Log($"[Inventory] Updated UI for slot {slot} with single item");
                    }
                }
            }
            else
            {
                // Slot is empty
                slotImages[slot].sprite = null;
                slotImages[slot].color = new Color(1, 1, 1, 0.5f);
                if (slotTexts != null && slotTexts[slot] != null)
                {
                    slotTexts[slot].text = "0";  // Show 0 for empty slots
                }
            }
        }
    }

    void Update()
    {
        // Handle number key input (1-3)
        for (int i = 0; i < inventory.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseItem(i);
            }
        }
    }

    private void UseItem(int slot)
    {
        if (inventory[slot] != null)
        {
            Debug.Log($"[Inventory] Attempting to use {inventory[slot].itemName} from slot {slot}");

            // Create a temporary item instance to use
            GameObject tempItem = new GameObject("TempItem");
            Item itemComponent = (Item)tempItem.AddComponent(inventory[slot].itemType);
            itemComponent.itemName = inventory[slot].itemName;
            itemComponent.itemSprite = inventory[slot].itemSprite;
            itemComponent.isStackable = inventory[slot].isStackable;
            itemComponent.isUsable = inventory[slot].isUsable;

            bool itemUsed = itemComponent.Use(gameObject);
            Destroy(tempItem);  // Clean up the temporary object

            if (itemUsed)
            {
                if (inventory[slot].isStackable && stackCounts[slot] > 1)
                {
                    stackCounts[slot]--;
                    UpdateSlotUI(slot);
                    Debug.Log($"[Inventory] Used {inventory[slot].itemName}. {stackCounts[slot]} remaining in slot {slot}");
                }
                else
                {
                    inventory[slot] = null;
                    stackCounts[slot] = 0;
                    UpdateSlotUI(slot);
                    Debug.Log($"[Inventory] Used last item from slot {slot}");
                }
            }
        }
    }

    public ItemData[] GetInventoryItems()
    {
        return inventory;
    }

    public bool HasItem(string itemName)
    {
        // Check each slot in the inventory
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null && inventory[i].itemName == itemName)
            {
                Debug.Log($"[Inventory] Found item: {itemName} in slot {i}");
                return true;
            }
        }
        Debug.Log($"[Inventory] Item not found: {itemName}");
        return false;
    }
}
