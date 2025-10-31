using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody2D rb;

    [Tooltip("Empties que definen las zonas de marchas")]
    public Transform zonaIzquierda; // Adelante
    public Transform zonaCentro;    // Neutro
    public Transform zonaDerecha;   // Atr?s

    [Header("Radios independientes de detecci?n (mundo)")]
    public float radioIzquierda = 0.9f;
    public float radioCentro = 0.7f;
    public float radioDerecha = 0.9f;

    [Header("Movimiento con 'feel' anterior")]
    [Tooltip("Fuerza de empuje (aceleraci?n). Se aplica hasta alcanzar vel. objetivo.")]
    public float fuerzaAceleracion = 20f;

    [Tooltip("Velocidad objetivo hacia adelante (m/s aprox).")]
    public float velMaxAdelante = 7f;

    [Tooltip("Velocidad objetivo marcha atr?s (m/s aprox).")]
    public float velMaxAtras = 4f;

    [Tooltip("Arrastre cuando se est? acelerando (m?s bajo = m?s inercia).")]
    public float dragEnMovimiento = 0.5f;

    [Tooltip("Arrastre en neutro (m?s alto = se frena antes).")]
    public float dragEnPuntoMuerto = 2.2f;

    [Tooltip("Suavizado extra en neutro (0 = nada, 10 = frena fuerte).")]
    [Range(0f, 10f)] public float neutralDamping = 3f;

    [Tooltip("Cu?nto alinear la velocidad hacia adelante/atr?s (reduce deriva lateral).")]
    [Range(0f, 1f)] public float alineacionVelocidad = 0.2f;

    [Header("Giro / Volante")]
    [Tooltip("Velocidad de rotaci?n en ?/s")]
    public float velocidadRotacion = 120f;

    [Tooltip("Multiplicador de rotaci?n cuando hay marcha (sensaci?n m?s ?gil).")]
    public float factorRotacionEnMovimiento = 1.0f;

    [Header("L?mites opcionales (clamp)")]
    public bool usarLimites = false;
    public Transform leftLimit, rightLimit, topLimit, bottomLimit;

    private enum Marcha { PuntoMuerto, Adelante, Atras }
    private Marcha marchaActual = Marcha.PuntoMuerto;

    private bool rotandoSostenido = false;
    private int sentidoRotacion = 1; // 1 = horario, -1 = antihorario
    private Camera cam;

    private void Reset() => rb = GetComponent<Rigidbody2D>();

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        // Setup f?sico ?cl?sico?
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = false;
        rb.drag = dragEnPuntoMuerto;
        rb.angularDrag = 0.5f;
    }

    void Update()
    {
        // Click: mientras est? PRESIONADO, gira. Cada nueva pulsaci?n invierte el sentido.
        if (Input.GetMouseButtonDown(0))
        {
            sentidoRotacion *= -1;
            rotandoSostenido = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            rotandoSostenido = false;
        }

        ActualizarMarchaPorMouse();
    }

    void FixedUpdate()
    {
        Vector2 forward = transform.up;
        float dt = Time.fixedDeltaTime;

        switch (marchaActual)
        {
            case Marcha.Adelante:
                rb.drag = dragEnMovimiento;
                AplicarEmpujeConTope(forward, velMaxAdelante, fuerzaAceleracion);
                AlinearVelocidad(forward, alineacionVelocidad);
                break;

            case Marcha.Atras:
                rb.drag = dragEnMovimiento;
                AplicarEmpujeConTope(-forward, velMaxAtras, fuerzaAceleracion);
                AlinearVelocidad(-forward, alineacionVelocidad);
                break;

            default: // Neutro
                rb.drag = dragEnPuntoMuerto;
                // Suavizado extra estilo ?coasting?
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, neutralDamping * dt);
                break;
        }

        // Rotaci?n
        if (rotandoSostenido)
        {
            float factor = (marchaActual == Marcha.PuntoMuerto) ? 1f : factorRotacionEnMovimiento;
            float deltaAng = velocidadRotacion * sentidoRotacion * factor * dt;
            rb.MoveRotation(rb.rotation + deltaAng);
        }

        // Clamp opcional
        if (usarLimites && leftLimit && rightLimit && topLimit && bottomLimit)
        {
            Vector2 p = rb.position;
            float x = Mathf.Clamp(p.x, leftLimit.position.x, rightLimit.position.x);
            float y = Mathf.Clamp(p.y, bottomLimit.position.y, topLimit.position.y);
            if (!Mathf.Approximately(x, p.x) || !Mathf.Approximately(y, p.y))
                rb.MovePosition(new Vector2(x, y));
        }
    }

    /// Aplica AddForce SOLO si la componente de velocidad en 'dirObjetivo' est? por debajo del tope.
    private void AplicarEmpujeConTope(Vector2 dirObjetivo, float velMax, float fuerza)
    {
        float vComp = Vector2.Dot(rb.velocity, dirObjetivo); // velocidad sobre el eje objetivo

        if (vComp < velMax)
        {
            rb.AddForce(dirObjetivo * fuerza, ForceMode2D.Force);
        }
        else
        {
            // Si se pasa por floating point, clipeamos suave a la velocidad tope
            Vector2 vel = rb.velocity;
            float exceso = vComp - velMax;
            if (exceso > 0f)
            {
                // resto la componente excedente sobre el eje objetivo
                vel -= dirObjetivo * exceso;
                rb.velocity = vel;
            }
        }
    }

    /// Corrige deriva lateral: reorienta gradualmente la velocidad hacia la direcci?n deseada.
    private void AlinearVelocidad(Vector2 dirObjetivo, float factor)
    {
        if (factor <= 0f) return;
        Vector2 v = rb.velocity;
        float mag = v.magnitude;
        if (mag < 0.001f) return;

        Vector2 vDeseada = dirObjetivo * mag * Mathf.Sign(Vector2.Dot(v, dirObjetivo));
        rb.velocity = Vector2.Lerp(v, vDeseada, factor);
    }

    private void ActualizarMarchaPorMouse()
    {
        if (!cam) cam = Camera.main;
        if (!cam) { marchaActual = Marcha.PuntoMuerto; return; }

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        bool sobreIzq = zonaIzquierda && (Vector2.Distance(mouseWorld, zonaIzquierda.position) <= radioIzquierda);
        bool sobreCen = zonaCentro && (Vector2.Distance(mouseWorld, zonaCentro.position) <= radioCentro);
        bool sobreDer = zonaDerecha && (Vector2.Distance(mouseWorld, zonaDerecha.position) <= radioDerecha);

        if (sobreIzq) marchaActual = Marcha.Adelante;
        else if (sobreCen) marchaActual = Marcha.PuntoMuerto;
        else if (sobreDer) marchaActual = Marcha.Atras;
        else marchaActual = Marcha.PuntoMuerto;
    }

    private void OnDrawGizmos()
    {
        // Gizmos de zonas
        DibujarZona(zonaIzquierda, radioIzquierda, new Color(0f, 1f, 0.2f, 0.9f));
        DibujarZona(zonaCentro, radioCentro, new Color(1f, 1f, 0.2f, 0.9f));
        DibujarZona(zonaDerecha, radioDerecha, new Color(1f, 0.2f, 0.2f, 0.9f));

        // Borde de l?mites
        if (usarLimites && leftLimit && rightLimit && topLimit && bottomLimit)
        {
            Gizmos.color = Color.cyan;
            Vector3 a = new Vector3(leftLimit.position.x, bottomLimit.position.y, 0f);
            Vector3 b = new Vector3(leftLimit.position.x, topLimit.position.y, 0f);
            Vector3 c = new Vector3(rightLimit.position.x, topLimit.position.y, 0f);
            Vector3 d = new Vector3(rightLimit.position.x, bottomLimit.position.y, 0f);
            Gizmos.DrawLine(a, b); Gizmos.DrawLine(b, c); Gizmos.DrawLine(c, d); Gizmos.DrawLine(d, a);
        }
    }

    private void DibujarZona(Transform t, float r, Color c)
    {
        if (!t) return;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(t.position, r);
    }
}
