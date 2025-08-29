using UnityEngine;

public class ZombieChase : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Target")]
    public Transform player;

    private Rigidbody2D rb;
    private ZombieHealth zombieHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        zombieHealth = GetComponent<ZombieHealth>();

        // Buscar jugador por tag si no está asignado
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
                Debug.LogError("❌ " + name + ": No se encontró el jugador (tag 'Player').");
        }

        if (rb == null)
            Debug.LogError("❌ " + name + ": Necesita Rigidbody2D.");
    }

    void Update()
    {
        // ✅ Si está en Stun, no hacer nada
        if (zombieHealth.IsStunned())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (player != null)
        {
            // 🎯 Mirar hacia el jugador
            Vector2 directionToPlayer = player.position - transform.position;
            transform.up = directionToPlayer; // El "frente" del zombie apunta al jugador

            // 🚶 Mover hacia el jugador
            rb.velocity = directionToPlayer.normalized * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}