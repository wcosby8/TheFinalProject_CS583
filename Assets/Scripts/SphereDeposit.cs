using UnityEngine;
using UnityEngine.Events;
using StarterAssets;

/// <summary>
/// Handles sphere deposits at the altar.
/// Player can deposit all their spheres at once by interacting with the altar.
/// </summary>
public class SphereDeposit : MonoBehaviour
{
    [Header("Deposit Settings")]
    [Tooltip("Interaction key to deposit spheres")]
    public KeyCode interactKey = KeyCode.E;
    
    [Tooltip("How close the player needs to be to interact (in units)")]
    public float interactionRange = 3f;
    
    [Header("Visual Feedback")]
    [Tooltip("Optional: GameObject to show when player is in range (e.g., 'Press E to Deposit')")]
    public GameObject interactionPrompt;
    
    [Tooltip("Optional: Particle effect to play when depositing")]
    public ParticleSystem depositEffect;
    
    [Tooltip("Optional: Light to turn on/glow when spheres are deposited")]
    public Light altarLight;
    
    [Header("Audio")]
    [Tooltip("Optional: Audio clip to play when depositing")]
    public AudioClip depositSound;
    
    private AudioSource audioSource;
    private PPlayerInventory playerInventory;
    private GameManager gameManager;
    private StarterAssetsInputs playerInputs;
    private bool playerInRange = false;
    
    [Header("Events")]
    public UnityEvent<int> OnSphereDeposited; // Passes number of spheres deposited
    public UnityEvent OnAllSpheresDeposited; // Called when all 6 spheres are deposited
    
    void Start()
    {
        // Get or create audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && depositSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Find player inventory & inputs
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<PPlayerInventory>();
            playerInputs = player.GetComponent<StarterAssetsInputs>();
        }
        
        // Find game manager
        gameManager = FindObjectOfType<GameManager>();
        
        // Hide interaction prompt initially
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void Update()
    {
        // Check if player is in range
        CheckPlayerInRange();
        
        // Handle interaction (new Input System via StarterAssetsInputs)
        if (playerInRange && playerInputs != null && playerInputs.interact)
        {
            AttemptDeposit();
            // consume the interact press so it doesn't fire every frame
            playerInputs.interact = false;
        }
    }
    
    /// <summary>
    /// Check if player is within interaction range
    /// </summary>
    void CheckPlayerInRange()
    {
        if (playerInventory == null) return;
        
        float distance = Vector3.Distance(transform.position, playerInventory.transform.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;
        
        // Show/hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(playerInRange && playerInventory.NumberOfSpheres > 0);
        }
        
        // Update prompt text if it has TextMeshPro or Text component
        if (playerInRange && playerInventory.NumberOfSpheres > 0)
        {
            UpdateInteractionPrompt();
        }
    }
    
    /// <summary>
    /// Update the interaction prompt text
    /// </summary>
    void UpdateInteractionPrompt()
    {
        if (interactionPrompt == null) return;
        
        // Try to find TextMeshPro component
        TMPro.TextMeshProUGUI tmpText = interactionPrompt.GetComponent<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = $"Press {interactKey} to Deposit ({playerInventory.NumberOfSpheres} spheres)";
            return;
        }
        
        // Try to find regular Text component
        UnityEngine.UI.Text text = interactionPrompt.GetComponent<UnityEngine.UI.Text>();
        if (text != null)
        {
            text.text = $"Press {interactKey} to Deposit ({playerInventory.NumberOfSpheres} spheres)";
        }
    }
    
    /// <summary>
    /// Attempt to deposit all spheres the player has
    /// </summary>
    public void AttemptDeposit()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("SphereDeposit: Player inventory not found!");
            return;
        }
        
        int spheresToDeposit = playerInventory.NumberOfSpheres;
        
        if (spheresToDeposit <= 0)
        {
            // No spheres to deposit
            return;
        }
        
        // Deposit all spheres
        DepositSpheres(spheresToDeposit);
    }
    
    /// <summary>
    /// Deposit a specific number of spheres
    /// </summary>
    public void DepositSpheres(int amount)
    {
        if (playerInventory == null) return;
        
        // Get actual amount (can't deposit more than player has)
        int actualAmount = Mathf.Min(amount, playerInventory.NumberOfSpheres);
        
        if (actualAmount <= 0) return;
        
        // Remove spheres from player inventory
        playerInventory.RemoveSpheres(actualAmount);
        
        // Add to game manager's deposited count
        if (gameManager != null)
        {
            gameManager.AddDepositedSpheres(actualAmount);
        }
        
        // Play effects
        PlayDepositEffects(actualAmount);
        
        // Invoke events
        OnSphereDeposited.Invoke(actualAmount);
        
        // Check if all spheres are deposited
        if (gameManager != null && gameManager.TotalDepositedSpheres >= 6)
        {
            OnAllSpheresDeposited.Invoke();
            if (gameManager != null)
            {
                gameManager.TriggerWin();
            }
        }
        
        Debug.Log($"Deposited {actualAmount} sphere(s)! Total deposited: {(gameManager != null ? gameManager.TotalDepositedSpheres : 0)}/6");
    }
    
    /// <summary>
    /// Play visual and audio effects when depositing
    /// </summary>
    void PlayDepositEffects(int amount)
    {
        // Play particle effect
        if (depositEffect != null)
        {
            depositEffect.Play();
        }
        
        // Play sound
        if (audioSource != null && depositSound != null)
        {
            audioSource.PlayOneShot(depositSound);
        }
        
        // Make altar light brighter (optional)
        if (altarLight != null)
        {
            altarLight.intensity = Mathf.Min(altarLight.intensity + 0.5f * amount, 5f);
        }
    }
    
    /// <summary>
    /// Draw gizmo to visualize interaction range in editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

