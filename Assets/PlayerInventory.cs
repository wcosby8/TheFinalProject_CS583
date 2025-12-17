using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int numberOfSpheres;   // This will show in Inspector

    public int NumberOfSpheres => numberOfSpheres;

    public UnityEvent<PlayerInventory> OnSphereCollected;

    public void SphereCollected()
    {
        numberOfSpheres++;
        OnSphereCollected.Invoke(this);
    }
}
