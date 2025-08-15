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
    public float weaponActiveTime = 0.3f;

    private bool isRotatingClockwise = true;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Vector2 lastDirection;

    // ✨ nuevo
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("Falta Rigidbody2D en Player.");
        else
        {
            rb.gravityScale = 0f;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        if (isAttacking) return;

        if (!isMoving)
            Rotate();

        if (Input.GetKeyDown(KeyCode.Space) && !isMoving && !isAttacking)
            StartMovement();

        if (Input.GetKeyUp(KeyCode.Space) && isMoving)
            StopMovementAndAttack();
    }

    void FixedUpdate()
    {
        if (isMoving && !isAttacking)
        {
            // ✅ mover con física para que respete colisiones
            Vector2 nextPos = rb.position + lastDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);
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

        if (weaponPrefab != null && attackPoint != null)
        {
            GameObject visualWeapon = Instantiate(weaponPrefab, attackPoint.position, attackPoint.rotation);
            Destroy(visualWeapon, weaponActiveTime);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 1.0f);
        foreach (Collider2D hit in hits)
        {
            ZombieHealth zombie = hit.GetComponent<ZombieHealth>();
            if (zombie != null)
                zombie.TakeDamage(20);
        }

        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        isAttacking = false;
        isRotatingClockwise = !isRotatingClockwise;
    }
}
