using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float speed = 10.0f;
    [SerializeField] public int damage = 10;
    public bool isPlayerBullet = false;
    public float lifetime = 3f;

    public Rigidbody rb;
    public bool hasHit = false;
    public GameObject shooter; // Reference to who shot this bullet

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Configure collider
        SphereCollider col = GetComponent<SphereCollider>();
        
        if (col != null)
        {
            col.isTrigger = true;
            Debug.Log("[Bullet] Collider configured: SphereCollider, isTrigger: True");
        }

        // Configure Rigidbody
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Debug.Log("[Bullet] Rigidbody configured - Gravity: False, CollisionDetection: Continuous");
        }

        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    void Start()
    {
        // Set initial velocity using Rigidbody
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        // Ignore collisions with the shooter
        if (other.gameObject == shooter)
        {
            Debug.Log("[Bullet] Ignored collision with shooter");
            return;
        }

        // If this is a player bullet, ignore collisions with the player
        if (isPlayerBullet && other.CompareTag("Player"))
        {
            Debug.Log("[Bullet] Player bullet ignored collision with player");
            return;
        }

        Debug.Log($"[Bullet] Collided with {other.gameObject.name} (Tag: {other.tag})");
        hasHit = true;

        // Handle player bullet hitting enemy
        if (isPlayerBullet && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"Hit enemy! Dealt {damage} damage");
            }
        }
        // Handle enemy bullet hitting player
        else if (!isPlayerBullet && other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Hit player! Dealt {damage} damage");
            }
        }

        Destroy(gameObject);
    }
}
