using UnityEngine;
using TMPro;
using System.Collections;

public class PickupTextManager : MonoBehaviour
{
    public TextMeshProUGUI pickupText;
    public float displayDuration = 2f;
    private Coroutine currentCoroutine;

    public void ShowPickupText(string itemName)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        
        currentCoroutine = StartCoroutine(ShowText(itemName));
    }

    IEnumerator ShowText(string text)
    {
        pickupText.text = $"Picked up: {text}";
        pickupText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(displayDuration);
        
        pickupText.gameObject.SetActive(false);
        currentCoroutine = null;
    }
}
