using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public Sprite itemSprite;  // Changed from itemIcon to itemSprite
    public bool isStackable;
    public bool isUsable;
    
    public abstract bool Use(GameObject player);  // Return true if item was used successfully
}
