using UnityEngine;

public class PatrolEnemy : Enemy
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;  // This is separate from baseSpeed
    public float minDistanceToPoint = 0.1f;
    
    [Header("Combat Settings")]
    public float contactDamageAmount = 10f;
    public float damageInterval = 0.5f;
    
    [Header("Debug Visualization")]
    public Color pathColor = Color.yellow;
    public float pointSize = 0.3f;
    
    public Transform currentTarget;
    public bool isInitialized = false;
    public float nextDamageTime = 0f;

    protected override void Start()
    {
        base.Start();
        
        if (pointA == null || pointB == null)
        {
            Debug.LogError("[PatrolEnemy] Patrol points not assigned!");
            enabled = false;
            return;
        }

        // Verify components
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("[PatrolEnemy] No Collider found! Please add a collider.");
        }
        
        if (GetComponent<Rigidbody>() == null)
        {
            Debug.LogError("[PatrolEnemy] No Rigidbody found! Please add a rigidbody.");
        }

        currentTarget = pointB;
        isInitialized = true;
        Debug.Log("[PatrolEnemy] Initialized with contact damage: " + contactDamage);
    }
    
    void Update()
    {
        if (!isInitialized) return;

        // Move towards current target
        transform.position = Vector3.MoveTowards(
            transform.position, 
            currentTarget.position, 
            patrolSpeed * Time.deltaTime
        );
        
        // Look at where we're going
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
        
        // Check if we reached the target
        if (Vector3.Distance(transform.position, currentTarget.position) < minDistanceToPoint)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        HandlePlayerCollision(collision);
    }

    protected override void OnCollisionStay(Collision collision)
    {
        HandlePlayerCollision(collision);
    }

    private void HandlePlayerCollision(Collision collision)
    {
        if (Time.time >= nextDamageTime && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("[PatrolEnemy] Collided with player!");
            
            HealthManager playerHealth = collision.gameObject.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log($"[PatrolEnemy] Dealt {contactDamage} damage to player!");
            }
            else
            {
                Debug.LogError("[PatrolEnemy] Player has no HealthManager component!");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
        }
    }
}
