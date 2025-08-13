using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Stun")]
    public float stunDuration = 0.3f; // Tiempo de aturdimiento

    private Rigidbody2D rb;
    private bool isStunned = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("❌ " + name + ": Necesita Rigidbody2D.");
        }
    }

    /// <summary>
    /// Recibe daño y entra en estado de Stun
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"💥 {name} recibió {damage} de daño. Vida: {currentHealth}");

        // Aplicar Stun
        EnterStun();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Activa el estado de Stun
    /// </summary>
    void EnterStun()
    {
        if (isStunned) return;

        isStunned = true;
        rb.bodyType = RigidbodyType2D.Kinematic; // Detiene movimiento y rotación
        rb.velocity = Vector2.zero;

        // Reactivar después del tiempo
        Invoke(nameof(ExitStun), stunDuration);
    }

    /// <summary>
    /// Termina el estado de Stun
    /// </summary>
    void ExitStun()
    {
        isStunned = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        Debug.Log($"🧟 {name} salió de Stun");
    }

    /// <summary>
    /// Para que otros scripts sepan si está aturdido
    /// </summary>
    public bool IsStunned()
    {
        return isStunned;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        CancelInvoke(); // Limpia el Invoke si se destruye
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}