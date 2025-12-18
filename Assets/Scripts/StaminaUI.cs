using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the stamina bar UI
/// </summary>
public class StaminaUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The stamina bar fill image (should be a child with Image component)")]
    public Image staminaBarFill;
    
    [Tooltip("Optional: The stamina bar background image")]
    public Image staminaBarBackground;
    
    [Tooltip("Maximum width of the stamina bar (will scale from 0 to this)")]
    public float maxBarWidth = 200f;
    
    [Header("Settings")]
    [Tooltip("Color when stamina is high (green)")]
    public Color highStaminaColor = Color.green;
    
    [Tooltip("Color when stamina is medium (yellow)")]
    public Color mediumStaminaColor = Color.yellow;
    
    [Tooltip("Color when stamina is low (red)")]
    public Color lowStaminaColor = Color.red;
    
    [Tooltip("Stamina threshold for medium color (0-1)")]
    [Range(0f, 1f)]
    public float mediumThreshold = 0.5f;
    
    [Tooltip("Stamina threshold for low color (0-1)")]
    [Range(0f, 1f)]
    public float lowThreshold = 0.25f;
    
    private StarterAssets.FirstPersonController playerController;
    private RectTransform fillRectTransform;
    private float originalWidth;
    
    void Start()
    {
        //find player controller
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<StarterAssets.FirstPersonController>();
        }
        
        //if no fill image assigned, try to find it
        if (staminaBarFill == null)
        {
            staminaBarFill = GetComponentInChildren<Image>();
        }
        
        //get RectTransform and store original width
        if (staminaBarFill != null)
        {
            fillRectTransform = staminaBarFill.GetComponent<RectTransform>();
            if (fillRectTransform != null)
            {
                originalWidth = fillRectTransform.sizeDelta.x;
                //if maxBarWidth is 0, use the current width
                if (maxBarWidth <= 0f)
                {
                    maxBarWidth = originalWidth;
                }
            }
        }
    }
    
    void Update()
    {
        if (playerController == null || staminaBarFill == null || fillRectTransform == null) return;
        
        //get stamina percentage
        float staminaPercent = playerController.StaminaPercentage;
        
        //update width based on stamina percentage
        float newWidth = maxBarWidth * staminaPercent;
        fillRectTransform.sizeDelta = new Vector2(newWidth, fillRectTransform.sizeDelta.y);
        
        //update color based on stamina level
        if (staminaPercent <= lowThreshold)
        {
            staminaBarFill.color = lowStaminaColor;
        }
        else if (staminaPercent <= mediumThreshold)
        {
            staminaBarFill.color = mediumStaminaColor;
        }
        else
        {
            staminaBarFill.color = highStaminaColor;
        }
    }
}

