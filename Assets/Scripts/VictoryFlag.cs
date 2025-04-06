using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class VictoryFlag : MonoBehaviour
{
    public Material victoryMaterial; // Material to show when victory is achieved
    public float rotationSpeed = 50f; // Make the flag rotate to draw attention
    public float delayBeforeEnd = 3f; // Time to wait after victory before ending game
    
    [Header("UI Settings")]
    public float winTextSize = 200f; // Large text size
    public Color winTextColor = Color.yellow; // Default to yellow color
    
    public MeshRenderer meshRenderer;
    public Material originalMaterial;
    public bool isActivated = false;
    public TextMeshProUGUI winText;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
        }
        
        // Create WIN text
        CreateWinText();
    }

    void CreateWinText()
    {
        // Create a new Canvas if it doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("VictoryCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create WIN text
        GameObject textObj = new GameObject("WinText");
        textObj.transform.SetParent(canvas.transform, false);
        
        winText = textObj.AddComponent<TextMeshProUGUI>();
        winText.text = "WIN";
        winText.fontSize = winTextSize;
        winText.color = winTextColor;
        winText.alignment = TextAlignmentOptions.Center;
        
        // Center the text
        RectTransform rectTransform = winText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Hide initially
        winText.enabled = false;
    }

    void Update()
    {
        // Rotate the flag to make it more noticeable
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public bool Activate(GameObject player)
    {
        if (!isActivated)
        {
            isActivated = true;
            Debug.Log("[VictoryFlag] Victory point activated!");
            
            
            if (victoryMaterial != null && meshRenderer != null)
            {
                meshRenderer.material = victoryMaterial;
            }

            
            InventoryManager inventory = player.GetComponent<InventoryManager>();
            if (inventory != null && inventory.HasItem("Key")) 
            {
                // Show victory messages
                if (winText != null)
                {
                    winText.enabled = true;
                    StartCoroutine(PulseWinText());
                }

                // Start the end game sequence
                StartCoroutine(EndGameSequence());
                return true;
            }
            else
            {
                Debug.Log("[VictoryFlag] Player does not have the key to win!");
                
            }
        }
        return false;
    }

    private IEnumerator PulseWinText()
    {
        float time = 0;
        while (time < delayBeforeEnd)
        {
            time += Time.deltaTime;
            float scale = 1 + Mathf.Sin(time * 4) * 0.1f; // Pulse between 0.9 and 1.1 scale
            winText.transform.localScale = new Vector3(scale, scale, 1);
            yield return null;
        }
    }

    private IEnumerator EndGameSequence()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeEnd);

        Debug.Log("[VictoryFlag] Ending game...");

        // In editor, stop playing
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // In build, quit application
            Application.Quit();
        #endif
    }
} 