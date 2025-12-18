using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PPlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int numberOfSpheres;   // This will show in Inspector

    public int NumberOfSpheres => numberOfSpheres;

    public UnityEvent<PPlayerInventory> OnSphereCollected;
    public UnityEvent<int> OnSpheresDeposited; // Passes number of spheres deposited
    public UnityEvent<SSphere> OnSphereStolen; // Passes the sphere that was stolen (for respawning)

    private SphereUI sphereUI;
    private List<SSphere> collectedSpheres = new List<SSphere>(); // Track collected spheres (LIFO - last in first out)

    void Start()
    {
        // Find sphere UI
        sphereUI = FindObjectOfType<SphereUI>();
    }

    public void SphereCollected(SSphere sphere, Vector3 originalPosition)
    {
        numberOfSpheres++;
        if (sphere != null)
        {
            collectedSpheres.Add(sphere); // Add to list (most recent at end)
        }
        OnSphereCollected.Invoke(this);
        
        // Show collection message
        if (sphereUI != null)
        {
            sphereUI.ShowCollectionMessage();
        }
    }
    
    /// <summary>
    /// Overload for backwards compatibility (if called without sphere reference)
    /// </summary>
    public void SphereCollected()
    {
        SphereCollected(null, Vector3.zero);
    }
    
    /// <summary>
    /// Remove spheres from inventory (when depositing or stolen)
    /// </summary>
    public void RemoveSpheres(int amount)
    {
        int actualAmount = Mathf.Min(amount, numberOfSpheres);
        numberOfSpheres -= actualAmount;
        OnSpheresDeposited.Invoke(actualAmount);
        // Also invoke OnSphereCollected to update UI (passes inventory reference)
        OnSphereCollected.Invoke(this);
    }
    
    /// <summary>
    /// Steal a sphere (removes from inventory and returns the sphere for respawning)
    /// </summary>
    public SSphere StealSphere()
    {
        if (numberOfSpheres <= 0 || collectedSpheres.Count == 0) return null;
        
        // Get the most recently collected sphere (last in list)
        SSphere stolenSphere = collectedSpheres[collectedSpheres.Count - 1];
        collectedSpheres.RemoveAt(collectedSpheres.Count - 1);
        
        numberOfSpheres--;
        OnSpheresDeposited.Invoke(1);
        OnSphereCollected.Invoke(this); // Update UI
        OnSphereStolen.Invoke(stolenSphere); // Notify for respawning
        
        return stolenSphere;
    }
}
