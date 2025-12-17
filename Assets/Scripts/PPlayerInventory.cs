using UnityEngine;
using UnityEngine.Events;

public class PPlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int numberOfSpheres;   // This will show in Inspector

    public int NumberOfSpheres => numberOfSpheres;

    public UnityEvent<PPlayerInventory> OnSphereCollected;
    public UnityEvent<int> OnSpheresDeposited; // Passes number of spheres deposited

    private SphereUI sphereUI;

    void Start()
    {
        // Find sphere UI
        sphereUI = FindObjectOfType<SphereUI>();
    }

    public void SphereCollected()
    {
        numberOfSpheres++;
        OnSphereCollected.Invoke(this);
        
        // Show collection message
        if (sphereUI != null)
        {
            sphereUI.ShowCollectionMessage();
        }
    }
    
    /// <summary>
    /// Remove spheres from inventory (when depositing)
    /// </summary>
    public void RemoveSpheres(int amount)
    {
        int actualAmount = Mathf.Min(amount, numberOfSpheres);
        numberOfSpheres -= actualAmount;
        OnSpheresDeposited.Invoke(actualAmount);
    }
}
