using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI sphereText;

    void Start()
    {
        sphereText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateSphereText(PlayerInventory playerInventory)
    {
        sphereText.text = playerInventory.NumberOfSpheres.ToString();
    }
}

