using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Manages UI messages for sphere collection and deposit
/// </summary>
public class SphereUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text element to show collection message (e.g., 'Deposit sphere to the altar')")]
    public TextMeshProUGUI collectionMessageText;
    
    [Tooltip("Text element to show deposit progress (e.g., '3/6 spheres deposited')")]
    public TextMeshProUGUI depositProgressText;
    
    [Tooltip("Text element to show persistent message (e.g., 'Deposit all 6 spheres to restore light to this world')")]
    public TextMeshProUGUI persistentMessageText;
    
    [Header("Settings")]
    [Tooltip("How long to show collection message (seconds)")]
    public float collectionMessageDuration = 3f;
    
    [Tooltip("How long to show deposit progress message (seconds)")]
    public float depositMessageDuration = 3f;
    
    [Tooltip("How long to show persistent message (seconds)")]
    public float persistentMessageDuration = 4f;
    
    [Tooltip("Fade in/out duration (seconds)")]
    public float fadeDuration = 0.5f;
    
    private GameManager gameManager;
    private Coroutine collectionMessageCoroutine;
    private Coroutine depositMessageCoroutine;
    private Coroutine persistentMessageCoroutine;
    
    void Start()
    {
        // Find game manager
        gameManager = FindObjectOfType<GameManager>();
        
        // Hide all messages initially
        if (collectionMessageText != null)
            collectionMessageText.gameObject.SetActive(false);
        
        if (depositProgressText != null)
            depositProgressText.gameObject.SetActive(false);
        
        // Hide persistent message initially
        if (persistentMessageText != null)
        {
            persistentMessageText.gameObject.SetActive(false);
            // Set alpha to 0 initially
            Color color = persistentMessageText.color;
            color.a = 0f;
            persistentMessageText.color = color;
        }
    }
    
    /// <summary>
    /// Show message when sphere is collected
    /// </summary>
    public void ShowCollectionMessage()
    {
        if (collectionMessageText == null) return;
        
        // Stop any existing coroutine
        if (collectionMessageCoroutine != null)
        {
            StopCoroutine(collectionMessageCoroutine);
        }
        
        // Show message
        collectionMessageText.text = "Deposit sphere to the altar";
        collectionMessageText.gameObject.SetActive(true);
        
        // Hide after duration
        collectionMessageCoroutine = StartCoroutine(HideMessageAfterDelay(collectionMessageText, collectionMessageDuration));
    }
    
    /// <summary>
    /// Show deposit progress message
    /// </summary>
    public void ShowDepositMessage(int deposited, int total)
    {
        if (depositProgressText == null) return;
        
        // Stop any existing coroutine
        if (depositMessageCoroutine != null)
        {
            StopCoroutine(depositMessageCoroutine);
        }
        
        // Show message
        depositProgressText.text = $"{deposited}/{total} spheres deposited";
        depositProgressText.gameObject.SetActive(true);
        
        // Hide after duration
        depositMessageCoroutine = StartCoroutine(HideMessageAfterDelay(depositProgressText, depositMessageDuration));
        
        // Show persistent message below (fade in/out)
        ShowPersistentMessage(deposited, total);
    }
    
    /// <summary>
    /// Show persistent message with fade in/out effect
    /// </summary>
    void ShowPersistentMessage(int deposited, int total)
    {
        if (persistentMessageText == null) return;
        
        // Stop any existing coroutine
        if (persistentMessageCoroutine != null)
        {
            StopCoroutine(persistentMessageCoroutine);
        }
        
        // Set message text based on progress
        if (deposited < total)
        {
            persistentMessageText.text = "Deposit all 6 spheres to restore light to this world";
        }
        else
        {
            persistentMessageText.text = "Light has been restored!";
        }
        
        // Start fade in/out coroutine
        persistentMessageCoroutine = StartCoroutine(FadeInOutMessage(persistentMessageText, persistentMessageDuration));
    }
    
    /// <summary>
    /// Coroutine to hide a message after a delay
    /// </summary>
    IEnumerator HideMessageAfterDelay(TextMeshProUGUI textElement, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (textElement != null)
        {
            textElement.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Coroutine to fade in, show, then fade out a message
    /// </summary>
    IEnumerator FadeInOutMessage(TextMeshProUGUI textElement, float showDuration)
    {
        if (textElement == null) yield break;
        
        // Make sure it's active
        textElement.gameObject.SetActive(true);
        
        // Fade in
        yield return StartCoroutine(FadeText(textElement, 0f, 1f, fadeDuration));
        
        // Show for duration
        yield return new WaitForSeconds(showDuration);
        
        // Fade out
        yield return StartCoroutine(FadeText(textElement, 1f, 0f, fadeDuration));
        
        // Deactivate
        if (textElement != null)
        {
            textElement.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Coroutine to fade text alpha from start to end over duration
    /// </summary>
    IEnumerator FadeText(TextMeshProUGUI textElement, float startAlpha, float endAlpha, float duration)
    {
        if (textElement == null) yield break;
        
        float elapsed = 0f;
        Color color = textElement.color;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            textElement.color = color;
            yield return null;
        }
        
        // Ensure final alpha
        color.a = endAlpha;
        textElement.color = color;
    }
}

