using UnityEngine;
using UnityEngine.Events;

public class PPlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int numberOfSpheres;   // This will show in Inspector

    public int NumberOfSpheres => numberOfSpheres;

    public UnityEvent<PPlayerInventory> OnSphereCollected;
    public UnityEvent<int> OnSpheresDeposited; // Passes number of spheres deposited

    public void SphereCollected()
    {
        numberOfSpheres++;
        OnSphereCollected.Invoke(this);
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
