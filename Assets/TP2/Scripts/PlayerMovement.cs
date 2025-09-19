
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 25f; // Velocidad de movimiento
    private bool verticalMode = false; // false = horizontal, true = vertical
    private Vector3 lastMousePos;
    private Vector3 movement;

    public Transform leftLimit;
    public Transform rightLimit;
    public Transform topLimit;
    public Transform bottomLimit;

    void Start()
    {
        lastMousePos = Input.mousePosition; // guardamos posición inicial del mouse
    }

    void Update()
    {
        // Cambiar modo con click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            verticalMode = !verticalMode;
            Debug.Log("Modo cambiado a: " + (verticalMode ? "Vertical" : "Horizontal"));
        }

        // Calcular delta del mouse
        Vector3 mouseDelta = Input.mousePosition - lastMousePos;
        lastMousePos = Input.mousePosition;

        // Dependiendo del modo, mapeamos el movimiento
        if (!verticalMode)
        {
            // Modo horizontal: mover en X
            movement = new Vector3(mouseDelta.x, 0f, 0f).normalized;
        }
        else
        {
            // Modo vertical: X del mouse controla arriba/abajo
            if (mouseDelta.x < 0)
                movement = Vector3.up;   // mover hacia arriba
            else if (mouseDelta.x > 0)
                movement = Vector3.down; // mover hacia abajo
            else
                movement = Vector3.zero;
        }

        // Aplicar movimiento
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Limitar posición dentro de los bordes
        float clampedX = Mathf.Clamp(transform.position.x, leftLimit.position.x, rightLimit.position.x);
        float clampedY = Mathf.Clamp(transform.position.y, bottomLimit.position.y, topLimit.position.y);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

}


