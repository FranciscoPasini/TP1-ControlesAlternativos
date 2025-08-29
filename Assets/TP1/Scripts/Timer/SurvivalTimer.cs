// SurvivalTimer.cs
using UnityEngine;
using UnityEngine.Events;

public class SurvivalTimer : MonoBehaviour
{
    [System.Serializable] public class TimeChangedEvent : UnityEvent<float> { }

    [Header("Referencias")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Eventos")]
    public TimeChangedEvent onTimeChanged;
    public UnityEvent<float> onTimerStopped;

    public float TimeAlive { get; private set; }
    private bool _running;

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        TimeAlive = 0f;
        _running = true;

        if (playerHealth != null)
            playerHealth.onDeath.AddListener(HandlePlayerDeath);
    }

    private void Update()
    {
        if (!_running) return;
        TimeAlive += Time.deltaTime;
        onTimeChanged?.Invoke(TimeAlive);
    }

    private void HandlePlayerDeath()
    {
        if (!_running) return;
        _running = false;
        onTimerStopped?.Invoke(TimeAlive);
    }

    public void ResetTimer()
    {
        _running = true;
        TimeAlive = 0f;
        onTimeChanged?.Invoke(TimeAlive);
    }
}
