using UnityEngine;

public class HealthBoostItem : Item
{
    void Awake()
    {
        itemName = "Health Boost";
        isStackable = true;
        isUsable = true;
    }

    public override bool Use(GameObject player)
    {
        Debug.Log("[HealthBoost] Attempting to use Health Boost");
        HealthManager healthManager = player.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            if (healthManager.currentHealth < healthManager.maxHealth)
            {
                healthManager.Heal(healthManager.maxHealth * 0.1f);
                Debug.Log($"[HealthBoost] Successfully healed 10% health. Current health: {healthManager.currentHealth}");
                return true;
            }
            else
            {
                Debug.Log("[HealthBoost] Cannot use - Health is already full!");
                return false;
            }
        }
        Debug.Log("[HealthBoost] Failed - No HealthManager found");
        return false;
    }
}
