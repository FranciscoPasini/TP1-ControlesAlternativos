using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseQuietDetector : MonoBehaviour
{
    public float tiempoQuietoNecesario = 3f;
    private Vector3 ultimaPosicionMouse;
    private float tiempoQuieto;

    [Header("Centro hacia donde se enviará la posición del mouse")]
    public Vector3 centroEscena = Vector3.zero;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }


    void Start()
    {
        ultimaPosicionMouse = Input.mousePosition;
    }

    void Update()
    {
        Vector3 posActual = Input.mousePosition;

        // ¿El mouse se movió?
        if ((posActual - ultimaPosicionMouse).sqrMagnitude < 0.01f)
        {
            tiempoQuieto += Time.deltaTime;
        }
        else
        {
            tiempoQuieto = 0f;
            ultimaPosicionMouse = posActual;
        }

        // ¿Lleva quieto 3 segundos?
        if (tiempoQuieto >= tiempoQuietoNecesario)
        {
            // Convertimos la posición del mouse al mundo
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(
                ultimaPosicionMouse.x,
                ultimaPosicionMouse.y,
                Mathf.Abs(Camera.main.transform.position.z)
            ));

            // “Enviar” al centro (haces lo que quieras con esto)
            mouseWorld = centroEscena;

            // Avanzar a la siguiente escena
            SceneManager.LoadScene("GameTP2");
        }
    }
}
