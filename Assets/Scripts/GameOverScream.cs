using UnityEngine;

/// <summary>
/// Plays the scream sound when the game over scene loads
/// </summary>
public class GameOverScream : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Scream audio clip (Staticheavy2)")]
    public AudioClip screamSound;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Get or create audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Play scream sound when scene loads
        if (screamSound != null)
        {
            audioSource.PlayOneShot(screamSound);
        }
        else
        {
            Debug.LogWarning("GameOverScream: No scream sound clip assigned!");
        }
    }
}

