using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    public float health = 100f;
    public float contactDamage = 10f;  
    protected float baseSpeed = 2f;     
    
    protected virtual void Start()
    {
        health = 100f;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var healthManager = collision.gameObject.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(contactDamage);
            }
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        // Base implementation
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"[Enemy] Took {damage} damage! Health: {health}");
        if (health <= 0)
        {
            Debug.Log("[Enemy] Health depleted, dying!");
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}