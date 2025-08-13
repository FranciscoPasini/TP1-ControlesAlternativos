using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement & Rotation")]
    public float rotationSpeed = 100f;
    public float moveSpeed = 5f;
    public float attackDuration = 0.5f;

    [Header("Attack Setup")]
    public Transform attackPoint;
    public GameObject weaponPrefab;
    public float weaponActiveTime = 0.3f; // Tiempo que el arma visible permanece antes de destruirse

    private bool isRotatingClockwise = true;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Vector2 lastDirection;

    void Update()
    {
        if (isAttacking) return;

        if (isMoving)
        {
            // Solo mover, sin rotar
        }
        else
        {
            Rotate(); // Rotar solo si no está moviendo ni atacando
        }

        // Entradas
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving && !isAttacking)
        {
            StartMovement();
        }

        if (Input.GetKeyUp(KeyCode.Space) && isMoving)
        {
            StopMovementAndAttack();
        }
    }

    void FixedUpdate()
    {
        if (isMoving && !isAttacking)
        {
            transform.position += (Vector3)lastDirection * moveSpeed * Time.fixedDeltaTime;
        }
    }

    void Rotate()
    {
        float direction = isRotatingClockwise ? -1f : 1f;
        transform.Rotate(0, 0, rotationSpeed * direction * Time.deltaTime);
    }

    void StartMovement()
    {
        isMoving = true;
        lastDirection = transform.up;
    }

    void StopMovementAndAttack()
    {
        isMoving = false;
        isAttacking = true;

        // ✅ 1. Instanciar el arma (solo visual)
        if (weaponPrefab != null && attackPoint != null)
        {
            GameObject visualWeapon = Instantiate(weaponPrefab, attackPoint.position, attackPoint.rotation);
            Destroy(visualWeapon, weaponActiveTime); // desaparece después
        }

        // ✅ 2. Detectar zombies en un radio alrededor del attackPoint
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 1.0f);
        foreach (Collider2D hit in hits)
        {
            ZombieHealth zombie = hit.GetComponent<ZombieHealth>();
            if (zombie != null)
            {
                // 🔥 Solo pasa el daño (sin knockback)
                zombie.TakeDamage(20);
            }
        }

        // ✅ 3. Finalizar ataque después de attackDuration
        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        isAttacking = false;
        isRotatingClockwise = !isRotatingClockwise; // Cambiar sentido de rotación
    }
}