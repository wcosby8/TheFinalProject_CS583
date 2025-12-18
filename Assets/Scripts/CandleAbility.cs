using UnityEngine;
using TMPro;

public class CandleAbility : MonoBehaviour
{
    [Header("Timing")]
    public float activeDuration = 15f;
    public float cooldownDuration = 30f;

    [Header("Light")]
    public LightSource playerCandleLightSource;

    [Header("UI")]
    public TextMeshProUGUI candleTimerText;

    private bool hasCandle = false;
    private bool isActive = false;
    private bool onCooldown = false;

    private float activeTimer;
    private float cooldownTimer;

    public bool CanUse => hasCandle && !isActive && !onCooldown;

    private void Awake()
    {
        if (playerCandleLightSource != null)
            playerCandleLightSource.SetLit(false);

        UpdateUIText();
    }

    private void Update()
    {
        if (isActive)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0f) EndActive();
            UpdateUIText();
        }
        else if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f) onCooldown = false;
            UpdateUIText();
        }
    }

    public void UnlockCandle()
    {
        hasCandle = true;
        UpdateUIText();
    }

    public void TryActivate()
    {
        if (!CanUse || playerCandleLightSource == null) return;

        isActive = true;
        activeTimer = activeDuration;
        playerCandleLightSource.SetLit(true);
        UpdateUIText();
    }

    private void EndActive()
    {
        isActive = false;
        playerCandleLightSource.SetLit(false);

        onCooldown = true;
        cooldownTimer = cooldownDuration;
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        if (candleTimerText == null) return;

        if (!hasCandle)
            candleTimerText.text = "Hint: Find a candle";
        else if (isActive)
            candleTimerText.text = $"Candle Active: {Mathf.CeilToInt(activeTimer)}s";
        else if (onCooldown)
            candleTimerText.text = $"Candle Cooldown: {Mathf.CeilToInt(cooldownTimer)}s";
        else
            candleTimerText.text = "Candle Ready";
    }
}
