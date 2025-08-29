// UIHud.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHud : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private SurvivalTimer survivalTimer;

    [Header("UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text timerText;

    private void Awake()
    {
        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (survivalTimer == null) survivalTimer = FindObjectOfType<SurvivalTimer>();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.AddListener(OnHealthChanged);

        if (survivalTimer != null)
        {
            survivalTimer.onTimeChanged.AddListener(OnTimeChanged);
            survivalTimer.onTimerStopped.AddListener(OnTimerStopped);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.RemoveListener(OnHealthChanged);

        if (survivalTimer != null)
        {
            survivalTimer.onTimeChanged.RemoveListener(OnTimeChanged);
            survivalTimer.onTimerStopped.RemoveListener(OnTimerStopped);
        }
    }

    private void Start()
    {
        if (playerHealth != null)
            OnHealthChanged(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        OnTimeChanged(0f);
    }

    private void OnHealthChanged(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }
    }

    private void OnTimeChanged(float seconds)
    {
        if (timerText == null) return;
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        int ms = Mathf.FloorToInt((seconds - Mathf.Floor(seconds)) * 1000f);
        timerText.text = $"{m:00}:{s:00}.{ms:000}";
    }

    private void OnTimerStopped(float finalTime)
    {
        OnTimeChanged(finalTime);
    }
}
