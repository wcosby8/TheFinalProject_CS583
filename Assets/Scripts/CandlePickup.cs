using UnityEngine;

public class CandlePickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CandleAbility ability = other.GetComponentInParent<CandleAbility>();
        if (ability == null) return;

        if (ability.CanUse)
        {
            ability.ActivateCandle();
            Destroy(gameObject);
        }
    }
}
