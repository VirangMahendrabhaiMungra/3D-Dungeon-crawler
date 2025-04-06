using UnityEngine;

public class MaxHealthItem : Item
{
    void Awake()
    {
        itemName = "Max Health";
        isStackable = false;
        isUsable = true;
    }

    public override bool Use(GameObject player)
    {
        Debug.Log("[MaxHealth] Attempting to use Max Health item");
        HealthManager healthManager = player.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            healthManager.SetFullHealth();
            Debug.Log("[MaxHealth] Successfully set health to maximum");
            return true;
        }
        Debug.Log("[MaxHealth] Failed - No HealthManager found");
        return false;
    }
}
