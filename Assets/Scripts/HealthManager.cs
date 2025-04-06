using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBar;
    public float delayBeforeEnd = 3f; // Time to wait before ending game

    [Header("UI Elements")]
    public TextMeshProUGUI loseText; // Reference to the LOSE text UI element
    public float loseTextSize = 200f;
    public Color loseTextColor = Color.red;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        CreateLoseText(); // Create the LOSE text UI
    }

    void CreateLoseText()
    {
        // Create a new Canvas if it doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("GameOverCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create LOSE text
        GameObject textObj = new GameObject("LoseText");
        textObj.transform.SetParent(canvas.transform, false);
        
        loseText = textObj.AddComponent<TextMeshProUGUI>();
        loseText.text = "LOSE";
        loseText.fontSize = loseTextSize;
        loseText.color = loseTextColor;
        loseText.alignment = TextAlignmentOptions.Center;
        
        // Center the text
        RectTransform rectTransform = loseText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Hide initially
        loseText.enabled = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"[HealthManager] Took {damage} damage! Health: {currentHealth}");
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthBar();
    }

    public void SetFullHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float healthPercentage = currentHealth / maxHealth;
            healthBar.fillAmount = healthPercentage;

            // Change color based on health percentage
            if (healthPercentage > 0.5f)
                healthBar.color = Color.green;
            else if (healthPercentage > 0.2f)
                healthBar.color = Color.yellow;
            else
                healthBar.color = Color.red;
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");

        // Show the lose text
        if (loseText != null)
        {
            loseText.enabled = true;
            StartCoroutine(PulseLoseText());
        }

        // Disable player movement and controls
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Disable shooting
        RayShooter rayShooter = GetComponentInChildren<RayShooter>();
        if (rayShooter != null)
        {
            rayShooter.enabled = false;
        }

        // Start end game sequence
        StartCoroutine(EndGameSequence());
    }

    private IEnumerator PulseLoseText()
    {
        float time = 0;
        while (time < delayBeforeEnd)
        {
            time += Time.deltaTime;
            float scale = 1 + Mathf.Sin(time * 4) * 0.1f; // Pulse between 0.9 and 1.1 scale
            loseText.transform.localScale = new Vector3(scale, scale, 1);
            yield return null;
        }
    }

    private IEnumerator EndGameSequence()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeEnd);

        Debug.Log("[HealthManager] Game Over - Player died");

        // In editor, stop playing
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // In build, quit application
            Application.Quit();
        #endif
    }
}
