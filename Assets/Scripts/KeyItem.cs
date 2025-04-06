using UnityEngine;

public class KeyItem : Item
{
    public float activationRange = 5f; // How far the key can be used from the victory flag

    void Awake()
    {
        itemName = "Key";
        isStackable = false;
        isUsable = true;
    }

    public override bool Use(GameObject player)
    {
        Debug.Log("[Key] Attempting to use key...");
        
        // Get the PickupTextManager once for all messages
        PickupTextManager textManager = player.GetComponent<PickupTextManager>();
        
        // Check for victory flag in range
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, activationRange);
        foreach (Collider collider in colliders)
        {
            VictoryFlag flag = collider.GetComponent<VictoryFlag>();
            if (flag != null)
            {
                if (flag.Activate(player))
                {
                    Debug.Log("[Key] Victory achieved!");
                    
                    if (textManager != null)
                    {
                        textManager.ShowPickupText("You've won! Key used successfully!");
                    }
                    
                    return true;
                }
            }
        }
        
        Debug.Log("[Key] No victory point in range! Get closer to the flag.");
        
        if (textManager != null)
        {
            textManager.ShowPickupText("Find the victory flag to use the key!");
        }
        
        return false;
    }
}
