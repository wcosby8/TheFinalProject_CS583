using UnityEngine;

/// <summary>
/// Permanent lantern/lamp post that wards off the demon doll.
/// These are static light sources placed around the map.
/// </summary>
public class Lantern : LightSource
{
    [Header("Lantern Settings")]
    [SerializeField] private bool startLit = true;  // Whether lantern starts lit
    
    protected override void Awake()
    {
        base.Awake();
        
        // Set initial lit state
        SetLit(startLit);
    }
    
    /// <summary>
    /// Can be called to toggle the lantern on/off (for gameplay mechanics)
    /// </summary>
    public void ToggleLantern()
    {
        SetLit(!isLit);
    }
    
    /// <summary>
    /// Light the lantern (if it was unlit)
    /// </summary>
    public void LightLantern()
    {
        if (!isLit)
        {
            SetLit(true);
            // Could add sound effect or particle effect here
        }
    }
    
    /// <summary>
    /// Extinguish the lantern (if it was lit)
    /// </summary>
    public void ExtinguishLantern()
    {
        if (isLit)
        {
            SetLit(false);
            // Could add sound effect or particle effect here
        }
    }
}

