// PlayerHealth.cs
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invulnerabilityTime = 0.4f;

    [Header("Eventos")]
    public UnityEvent<int, int> onHealthChanged; // (current, max)
    public UnityEvent onDeath;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    private bool _invulnerable;
    private float _invulnUntil;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Update()
    {
        if (_invulnerable && Time.time >= _invulnUntil)
            _invulnerable = false;
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        if (_invulnerable) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
        {
            onDeath?.Invoke();
        }
        else
        {
            _invulnerable = true;
            _invulnUntil = Time.time + invulnerabilityTime;
        }
    }

    public void ResetHealth()
    {
        _invulnerable = false;
        CurrentHealth = maxHealth;
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
