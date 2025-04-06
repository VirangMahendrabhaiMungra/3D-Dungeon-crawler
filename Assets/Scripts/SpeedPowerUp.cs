using UnityEngine;

public class SpeedPowerUp : Item
{
    public float speedMultiplier = 2f;
    public float duration = 10f;

    void Awake()
    {
        itemName = "Speed Boost";
        isStackable = false;
        isUsable = true;
    }

    public override bool Use(GameObject player)
    {
        Debug.Log("[SpeedBoost] Attempting to use Speed Boost");
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ApplySpeedBoost(speedMultiplier, duration);
            Debug.Log("[SpeedBoost] Speed boost applied successfully");
            return true;
        }
        Debug.Log("[SpeedBoost] Failed - No PlayerController found");
        return false;
    }
}
