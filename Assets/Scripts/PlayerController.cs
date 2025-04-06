using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 3.0f;
    public float sensitivity = 1.0f;
    public float jumpForce = 5f;
    
    public CharacterController characterController;
    public Camera playerCamera;
    public float verticalRotation = 0f;
    public Vector3 verticalVelocity = Vector3.zero;
    public float gravity = -9.81f;
    public Coroutine speedBoostCoroutine;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle Movement
        float moveForward = Input.GetAxis("Vertical") * speed;
        float moveSide = Input.GetAxis("Horizontal") * speed;
        
        Vector3 movement = transform.right * moveSide + transform.forward * moveForward;

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity.y = -1f; // Small downward force when grounded
            
            // Jump
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity.y = jumpForce;
            }
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        // Apply vertical velocity to movement
        movement += verticalVelocity * Time.deltaTime;
        
        // Move the character
        characterController.Move(movement * Time.deltaTime);

        // ONLY rotate when there's actual mouse input
        float mouseX = Input.GetAxis("Mouse X");
        if (mouseX != 0)
        {
            transform.Rotate(0, mouseX * sensitivity, 0);
        }

        // Camera vertical rotation
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseY != 0)
        {
            verticalRotation -= mouseY * sensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }
        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        Debug.Log($"[SpeedBoost] Speed increased by {multiplier}x for {duration} seconds");
        float originalSpeed = speed;
        speed *= multiplier;

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
        Debug.Log("[SpeedBoost] Speed returned to normal");
        speedBoostCoroutine = null;
    }
}
