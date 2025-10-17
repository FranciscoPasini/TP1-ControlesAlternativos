using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 25f;
    private bool verticalMode = false;
    private Vector3 lastMousePos;
    private Vector3 movement;

    public Transform leftLimit;
    public Transform rightLimit;
    public Transform topLimit;
    public Transform bottomLimit;

    public LayerMask wallLayer; // asigná la capa "Wall" en el inspector

    void Start()
    {
        lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            verticalMode = !verticalMode;

        Vector3 mouseDelta = Input.mousePosition - lastMousePos;
        lastMousePos = Input.mousePosition;

        if (!verticalMode)
            movement = new Vector3(mouseDelta.x, 0f, 0f).normalized;
        else
            movement = mouseDelta.x < 0 ? Vector3.up : mouseDelta.x > 0 ? Vector3.down : Vector3.zero;

        // Calcular siguiente posición
        Vector3 targetPos = transform.position + movement * moveSpeed * Time.deltaTime;

        // Verificar colisión con paredes antes de moverse
        if (!Physics2D.OverlapBox(targetPos, new Vector2(0.5f, 0.5f), 0f, wallLayer))
        {
            transform.position = targetPos;
        }

        // Limitar dentro de los bordes
        float clampedX = Mathf.Clamp(transform.position.x, leftLimit.position.x, rightLimit.position.x);
        float clampedY = Mathf.Clamp(transform.position.y, bottomLimit.position.y, topLimit.position.y);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    // Visualizar la caja de detección en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.5f, 0.5f, 0f));
    }
}
