using UnityEngine;
using System.Collections;

public class RayShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float damage = 10f;
    public ParticleSystem bulletImpact; 
    
    [Header("Audio")]
    public AudioSource audioSource;        // Reference to the AudioSource component
    public AudioClip shootSound;          // Sound played when firing
    public AudioClip bulletTravelSound;   
    
    private Camera cam;
    public InventoryManager inventory;
    public bool hasGun = false;
    public int gunSlot = -1;

    void Start()
    {
        cam = GetComponent<Camera>();
        inventory = transform.parent.GetComponent<InventoryManager>();
        
        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Check if we have a gun
        if (!hasGun)
        {
            CheckForGun();
        }

        // Only shoot if we have a gun
        if (hasGun && Input.GetMouseButtonDown(0))
        {
            Debug.Log("[RayShooter] Shoot button pressed!");
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("[RayShooter] Bullet prefab is not assigned!");
            return;
        }

        // Play shoot sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Calculate spawn position - move it forward from the camera to avoid self-collision
        Vector3 spawnPos;
        if (shootPoint != null)
        {
            spawnPos = shootPoint.position;
        }
        else
        {
            // If no shoot point, spawn bullet 1 unit in front of camera
            spawnPos = cam.transform.position + cam.transform.forward * 1.0f;
        }
        
        Vector3 direction = cam.transform.forward;
        
        Debug.Log($"[RayShooter] Spawning bullet at {spawnPos}, direction: {direction}");
        
        // Create bullet with the correct rotation
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(direction));
        
        // Configure bullet
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.isPlayerBullet = true;
            bulletScript.SetShooter(transform.parent.gameObject); // Set the player as the shooter
            
            
            if (bulletTravelSound != null)
            {
                AudioSource bulletAudio = bullet.AddComponent<AudioSource>();
                bulletAudio.clip = bulletTravelSound;
                bulletAudio.loop = true;  
                bulletAudio.spatialBlend = 1f;  
                bulletAudio.minDistance = 1f;
                bulletAudio.maxDistance = 20f;
                bulletAudio.Play();
            }
        }
        
        // Set bullet velocity
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = direction * 20f; // Set a fixed speed
        }
    }

    void CheckForGun()
    {
        if (inventory != null)
        {
            ItemData[] items = inventory.GetInventoryItems();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && items[i].itemType == typeof(GunItem))
                {
                    hasGun = true;
                    gunSlot = i;
                    Debug.Log("[RayShooter] Found gun in inventory slot " + i);
                    break;
                }
            }
        }
    }

    void OnGUI()
    {
        
        if (hasGun)
        {
            int size = 45;
            float posX = cam.pixelWidth/2 - size/4;
            float posY = cam.pixelHeight/2 - size/2;
            GUI.Label(new Rect(posX, posY, size, size), "+");
        }
    }
}
