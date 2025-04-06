using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public Sprite itemSprite;
    public System.Type itemType;
    public bool isStackable;
    public bool isUsable;

    public ItemData(Item item)
    {
        this.itemName = item.itemName;
        this.itemSprite = item.itemSprite;
        this.itemType = item.GetType();
        this.isStackable = item.isStackable;
        this.isUsable = item.isUsable;
    }
}
