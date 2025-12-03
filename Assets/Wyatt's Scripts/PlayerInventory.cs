using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int numberOfSpheres;   // This will show in Inspector

    public int NumberOfSpheres => numberOfSpheres;

    public void SphereCollected()
    {
        numberOfSpheres++;
    }
}

