using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody2D rb;

    [Tooltip("Empties que definen las zonas de marchas")]
    public Transform zonaIzquierda; // Adelante
    public Transform zonaCentro;    // Neutro
    public Transform zonaDerecha;   // Atrás

    [Header("Tamaños independientes de detección (mundo)")]
    public Vector2 sizeIzquierda = new Vector2(1.8f, 1.8f);
    public Vector2 sizeCentro = new Vector2(1.4f, 1.4f);
    public Vector2 sizeDerecha = new Vector2(1.8f, 1.8f);

    [Header("Movimiento con 'feel' anterior")]
    [Tooltip("Fuerza de empuje (aceleración). Se aplica hasta alcanzar vel. objetivo.")]
    public float fuerzaAceleracion = 20f;

    [Tooltip("Velocidad objetivo hacia adelante (m/s aprox).")]
    public float velMaxAdelante = 7f;

    [Tooltip("Velocidad objetivo marcha atrás (m/s aprox).")]
    public float velMaxAtras = 1f;

    [Tooltip("Arrastre cuando se está acelerando (más bajo = más inercia).")]
    public float dragEnMovimiento = 0.5f;

    [Tooltip("Arrastre en neutro (más alto = se frena antes).")]
    public float dragEnPuntoMuerto = 2.2f;

    [Tooltip("Suavizado extra en neutro (0 = nada, 10 = frena fuerte).")]
    [Range(0f, 10f)] public float neutralDamping = 3f;

    [Tooltip("Cuánto alinear la velocidad hacia adelante/atrás (reduce deriva lateral).")]
    [Range(0f, 1f)] public float alineacionVelocidad = 0.2f;

    [Header("Giro / Volante")]
    [Tooltip("Velocidad de rotación en °/s")]
    public float velocidadRotacion = 120f;

    [Tooltip("Multiplicador de rotación cuando hay marcha (sensación más ágil).")]
    public float factorRotacionEnMovimiento = 1.0f;

    [Header("Límites opcionales (clamp)")]
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

        // Setup físico "clásico"
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = false;
        rb.drag = dragEnPuntoMuerto;
        rb.angularDrag = 0.5f;
    }

    void Update()
    {
        // Click: mientras está PRESIONADO, gira. Cada nueva pulsación invierte el sentido.
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
                // Suavizado extra estilo "coasting"
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, neutralDamping * dt);
                break;
        }

        // Rotación
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

    /// Aplica AddForce SOLO si la componente de velocidad en 'dirObjetivo' está por debajo del tope.
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

    /// Corrige deriva lateral: reorienta gradualmente la velocidad hacia la dirección deseada.
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

        Vector3 mouseWorld3 = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseWorld = new Vector2(mouseWorld3.x, mouseWorld3.y);

        bool sobreIzq = PointInBox(zonaIzquierda, sizeIzquierda, mouseWorld);
        bool sobreCen = PointInBox(zonaCentro, sizeCentro, mouseWorld);
        bool sobreDer = PointInBox(zonaDerecha, sizeDerecha, mouseWorld);

        if (sobreIzq) marchaActual = Marcha.Adelante;
        else if (sobreCen) marchaActual = Marcha.PuntoMuerto;
        else if (sobreDer) marchaActual = Marcha.Atras;
        else marchaActual = Marcha.PuntoMuerto;
    }

    /// Comprueba si un punto en mundo está dentro de la caja definida por el transform (centro/rotación/escala)
    /// y el tamaño en unidades del mundo.
    private bool PointInBox(Transform t, Vector2 size, Vector2 pointWorld)
    {
        if (!t) return false;

        // Llevar punto al espacio local del transform (respeta rotación y escala)
        Vector3 local = t.InverseTransformPoint(new Vector3(pointWorld.x, pointWorld.y, t.position.z));
        // Consideramos la escala del transform aplicada al tamaño
        Vector2 scaledSize = new Vector2(size.x * t.lossyScale.x, size.y * t.lossyScale.y);
        Vector2 half = scaledSize * 0.5f;

        return (Mathf.Abs(local.x) <= half.x + Mathf.Epsilon) && (Mathf.Abs(local.y) <= half.y + Mathf.Epsilon);
    }

    private void OnDrawGizmos()
    {
        // Gizmos de zonas (cajas orientadas)
        DibujarCaja(zonaIzquierda, sizeIzquierda, new Color(0f, 1f, 0.2f, 0.9f));
        DibujarCaja(zonaCentro, sizeCentro, new Color(1f, 1f, 0.2f, 0.9f));
        DibujarCaja(zonaDerecha, sizeDerecha, new Color(1f, 0.2f, 0.2f, 0.9f));

        // Borde de límites
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

    private void DibujarCaja(Transform t, Vector2 size, Color c)
    {
        if (!t) return;
        Gizmos.color = c;

        // Guardamos la matriz anterior
        Matrix4x4 prev = Gizmos.matrix;

        // Construimos una matriz TRS para dibujar el cubo en el espacio del transform,
        // respetando posición, rotación y escala (ajustamos la escala Z a 1).
        Vector3 lossyScale = t.lossyScale;
        Vector3 scale = new Vector3(size.x * lossyScale.x, size.y * lossyScale.y, 1f);

        Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, scale);

        // Dibujamos un wire-cube centrado en 0 con tamaño 1x1 (la matriz escala a la medida real)
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        // Restauramos la matriz anterior
        Gizmos.matrix = prev;
    }
}
