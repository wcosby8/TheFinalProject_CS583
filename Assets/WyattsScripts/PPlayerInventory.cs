using UnityEngine;
using UnityEngine.Events;

public class PPlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int numberOfSpheres;   // This will show in Inspector

    public int NumberOfSpheres => numberOfSpheres;

    public UnityEvent<PPlayerInventory> OnSphereCollected;

    public void SphereCollected()
    {
        numberOfSpheres++;
        OnSphereCollected.Invoke(this);
    }
}
