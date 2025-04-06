using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WanderEnemy : Enemy
{
    [Header("Movement")]
    public float wanderRadius = 10f;
    public float minWanderDistance = 2f;
    
    [Header("Combat")]
    public float sightRange = 10f;
    public float shootRange = 8f;
    public float shootCooldown = 2f;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip bulletTravelSound;

    public NavMeshAgent agent;
    public Vector3 wanderTarget;
    public float nextShootTime;
    public Transform player;
    public bool isAlive = true;
    public bool destinationSet = false;

    protected override void Start()
    {
        base.Start();
        
        // Get and configure NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("[WanderEnemy] NavMeshAgent component missing!");
            enabled = false;
            return;
        }
        agent.speed = baseSpeed;
        agent.stoppingDistance = 0.5f;

        // Find player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("[WanderEnemy] No player found in scene!");
        }

        // Set shoot point
        if (shootPoint == null) 
            shootPoint = transform;

        // Initial wander point
        SetNewWanderPoint();

        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (!isAlive || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is in range and line of sight
        if (distanceToPlayer <= sightRange && HasLineOfSightToPlayer())
        {
            // Stop moving and face player
            agent.isStopped = true;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                                                Quaternion.LookRotation(directionToPlayer), 
                                                5f * Time.deltaTime);

            // Shoot if in range
            if (distanceToPlayer <= shootRange && Time.time >= nextShootTime)
            {
                Shoot(player.position);
                nextShootTime = Time.time + shootCooldown;
            }
            }
            else
            {
            // Resume wandering
            agent.isStopped = false;
            Wander();
        }
    }

    void Wander()
    {
        // If we've reached the destination or don't have one, get a new point
        if (!destinationSet || agent.remainingDistance < minWanderDistance)
                {
                    SetNewWanderPoint();
        }
    }

    void SetNewWanderPoint()
    {
        // Try to find a valid point
        for (int i = 0; i < 30; i++) // Limit attempts to prevent infinite loop
        {
            // Get random direction and distance
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;

            // Sample a point on the NavMesh
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                wanderTarget = hit.position;
                agent.SetDestination(wanderTarget);
                destinationSet = true;
                Debug.Log($"[WanderEnemy] New wander point set: {wanderTarget}");
                return;
            }
        }
        
        Debug.LogWarning("[WanderEnemy] Failed to find valid wander point!");
        destinationSet = false;
    }

    bool HasLineOfSightToPlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit))
        {
            return hit.transform == player;
        }
        return false;
    }

    void Shoot(Vector3 targetPosition)
    {
        if (bulletPrefab == null) return;

        
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        
        Vector3 spawnPos = shootPoint != null ? 
            shootPoint.position : 
            transform.position + transform.forward * 1.0f;
        
        // Calculate direction to player's center mass
        Vector3 targetCenter = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z); // Aim at player's body
        Vector3 direction = (targetCenter - spawnPos).normalized;
        
        
        Debug.Log($"[WanderEnemy] Enemy pos: {transform.position}, Player pos: {targetPosition}, Aim point: {targetCenter}");
        
        // Check for obstacles between enemy and player
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, direction, out hit, shootRange))
        {
            // If we hit something that's not the player, don't shoot
            if (!hit.collider.CompareTag("Player"))
            {
                Debug.Log($"[WanderEnemy] Shot blocked by {hit.collider.name} at distance {hit.distance}");
                return;
            }
        }
        
        // Add minimal spread
        float spread = 0.05f;
        direction += new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
        );
        direction.Normalize();
        
        // Create and configure bullet
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(direction));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.isPlayerBullet = false;
            bulletScript.SetShooter(gameObject);
            bulletScript.damage = 10;
            
            // Add travel sound if provided
            if (bulletTravelSound != null)
            {
                AudioSource bulletAudio = bullet.AddComponent<AudioSource>();
                bulletAudio.clip = bulletTravelSound;
                bulletAudio.loop = true;
                bulletAudio.spatialBlend = 1f;  // Make it 3D sound
                bulletAudio.minDistance = 1f;
                bulletAudio.maxDistance = 20f;
                bulletAudio.Play();
            }
        }
        
        // Set bullet velocity
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            float bulletSpeed = 20f;
            bulletRb.velocity = direction * bulletSpeed;
        }

        Debug.Log($"[WanderEnemy] Shot fired at player! Direction: {direction}");
    }

    public void SetAlive(bool alive)
    {
        isAlive = alive;
        if (!alive)
        {
            if (agent != null)
                agent.isStopped = true;
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        // Rotate to fall over
        float elapsed = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(-90, 0, 0));
        
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / 0.5f);
            yield return null;
        }
        
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Draw ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        
        // Draw current target
        if (Application.isPlaying && destinationSet)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wanderTarget, 0.5f);
            Gizmos.DrawLine(transform.position, wanderTarget);
        }
        
        // Draw shoot point
        if (shootPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shootPoint.position, 0.2f);
            Gizmos.DrawRay(shootPoint.position, shootPoint.forward * 2f);
        }
        
        // Draw aim visualization if in play mode
        if (Application.isPlaying && player != null)
        {
            Vector3 targetCenter = player.position; // Removed height offset
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(shootPoint != null ? shootPoint.position : transform.position, targetCenter);
            Gizmos.DrawWireSphere(targetCenter, 0.2f);
        }
    }
}
