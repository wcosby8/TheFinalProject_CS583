using UnityEngine;

/// <summary>
/// Base class for all light sources that can ward off the demon doll.
/// Provides shared functionality for light detection and radius management.
/// </summary>
public abstract class LightSource : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] protected float lightRadius = 10f;  // How far the light reaches
    [SerializeField] protected bool isLit = true;        // Whether this light is currently active
    
    [Header("Visual Feedback")]
    [SerializeField] protected Light pointLight;         // Unity Light component (optional)
    [SerializeField] protected GameObject lightVisual;    // Visual representation (glow, particles, etc.)
    
    public float LightRadius => lightRadius;
    public bool IsLit => isLit;
    
    protected virtual void Awake()
    {
        // Try to find a Light component if not assigned
        if (pointLight == null)
            pointLight = GetComponentInChildren<Light>();
        
        UpdateLightVisuals();
    }
    
    /// <summary>
    /// Check if a position is within this light's radius
    /// </summary>
    public bool IsPositionInLight(Vector3 position)
    {
        if (!isLit) return false;
        
        float distance = Vector3.Distance(transform.position, position);
        return distance <= lightRadius;
    }
    
    /// <summary>
    /// Get the distance from a position to the edge of this light's radius
    /// Returns negative if inside the radius, positive if outside
    /// </summary>
    public float GetDistanceToLightEdge(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        return distance - lightRadius;
    }
    
    /// <summary>
    /// Set whether this light is lit or not
    /// </summary>
    public virtual void SetLit(bool lit)
    {
        isLit = lit;
        UpdateLightVisuals();
    }
    
    /// <summary>
    /// Update visual representation of the light
    /// </summary>
    protected virtual void UpdateLightVisuals()
    {
        if (pointLight != null)
            pointLight.enabled = isLit;
        
        if (lightVisual != null)
            lightVisual.SetActive(isLit);
    }
    
    /// <summary>
    /// Draw gizmo in editor to visualize light radius
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = isLit ? Color.yellow : Color.gray;
        Gizmos.DrawWireSphere(transform.position, lightRadius);
    }
}

