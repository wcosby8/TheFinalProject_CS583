using UnityEngine;
using TMPro;

public class IInventoryUI : MonoBehaviour
{
    private TextMeshProUGUI sphereText;

    void Start()
    {
        sphereText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateSphereText(PPlayerInventory playerInventory)
    {
        sphereText.text = playerInventory.NumberOfSpheres.ToString();
    }
}
