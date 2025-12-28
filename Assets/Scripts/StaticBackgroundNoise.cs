using UnityEngine;

/// <summary>
/// Plays static background noise that loops continuously throughout the game
/// </summary>
public class StaticBackgroundNoise : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Static background noise audio clip (should loop)")]
    public AudioClip staticLightNoise;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Get or create audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure audio source for looping
        audioSource.clip = staticLightNoise;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        
        // Play the static noise
        if (staticLightNoise != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("StaticBackgroundNoise: No static light noise clip assigned!");
        }
    }
}

