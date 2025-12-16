using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages game state, win condition, and sphere deposit tracking
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Win Condition")]
    [Tooltip("Total number of spheres needed to win")]
    public int spheresNeededToWin = 6;
    
    [SerializeField]
    private int totalDepositedSpheres = 0;
    
    public int TotalDepositedSpheres => totalDepositedSpheres;
    
    [Header("Win Screen")]
    [Tooltip("Optional: GameObject to show when player wins (e.g., UI panel)")]
    public GameObject winScreen;
    
    [Tooltip("Optional: Audio clip to play when winning")]
    public AudioClip winSound;
    
    [Header("Events")]
    public UnityEvent OnWinConditionMet;
    
    private AudioSource audioSource;
    private bool hasWon = false;
    
    void Start()
    {
        // Get or create audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && winSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Hide win screen initially
        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
    }
    
    /// <summary>
    /// Add spheres to the deposited count
    /// </summary>
    public void AddDepositedSpheres(int amount)
    {
        totalDepositedSpheres += amount;
        totalDepositedSpheres = Mathf.Min(totalDepositedSpheres, spheresNeededToWin);
        
        Debug.Log($"Total deposited spheres: {totalDepositedSpheres}/{spheresNeededToWin}");
    }
    
    /// <summary>
    /// Check if win condition is met and trigger win
    /// </summary>
    public void TriggerWin()
    {
        if (hasWon) return; // Prevent multiple win triggers
        
        if (totalDepositedSpheres >= spheresNeededToWin)
        {
            hasWon = true;
            OnWinConditionMet.Invoke();
            ShowWinScreen();
            PlayWinSound();
            
            Debug.Log("YOU WIN! All spheres deposited!");
        }
    }
    
    /// <summary>
    /// Show the win screen
    /// </summary>
    void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }
    }
    
    /// <summary>
    /// Play win sound
    /// </summary>
    void PlayWinSound()
    {
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }
    }
    
    /// <summary>
    /// Restart the game (can be called from UI button)
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Quit the game (can be called from UI button)
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

