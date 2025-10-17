using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    public List<GameObject> layouts; // Lista de layouts en el orden que deben activarse
    private int currentLayoutIndex = 0;

    [Header("Resolución de overlaps")]
    [Tooltip("Referencia al transform del player (arrastrar)")]
    public Transform playerTransform;

    [Tooltip("Collider del player (arrastrar) — usado para conocer tamaño)")]
    public Collider2D playerCollider;

    [Tooltip("Capa de las paredes (solo para comprobaciones)")]
    public LayerMask wallLayer;

    [Tooltip("Máxima distancia (en unidades) para intentar sacar al player)")]
    public float maxResolveDistance = 3f;

    [Tooltip("Paso de búsqueda radial (en unidades)")]
    public float resolveStep = 0.2f;

    private void Start()
    {
        // Desactivar todos los layouts al inicio
        foreach (var layout in layouts)
            if (layout != null) layout.SetActive(false);

        // Activar el primero
        if (layouts.Count > 0 && layouts[0] != null)
            layouts[0].SetActive(true);
    }

    public void OnScoreChanged(int score)
    {
        // Cada 5 puntos, cambiamos de layout
        int newIndex = Mathf.FloorToInt(score / 5f);

        // Si hay un layout nuevo disponible
        if (newIndex != currentLayoutIndex && newIndex < layouts.Count)
        {
            // Desactivar el anterior
            if (currentLayoutIndex < layouts.Count && layouts[currentLayoutIndex] != null)
                layouts[currentLayoutIndex].SetActive(false);

            // Activar el nuevo
            if (layouts[newIndex] != null)
                layouts[newIndex].SetActive(true);

            currentLayoutIndex = newIndex;
            Debug.Log("Cambiado al layout " + newIndex);

            // Intentar resolver si el player queda superpuesto con paredes
            TryResolvePlayerOverlap();
        }
    }

    private void TryResolvePlayerOverlap()
    {
        if (playerTransform == null || playerCollider == null)
        {
            Debug.LogWarning("LayoutManager: falta playerTransform o playerCollider para resolver overlaps.");
            return;
        }

        // Si no hay overlap en la posición actual, no hacemos nada
        if (!IsOverlappingWalls(playerTransform.position))
            return;

        // Búsqueda radial: probamos posiciones alrededor del jugador
        Vector2 origin = playerTransform.position;
        float maxD = maxResolveDistance;
        float step = resolveStep;

        // Primero intentamos acercamientos en cuadrados crecientes (grid), luego radial
        for (float r = step; r <= maxD; r += step)
        {
            // probar varias direcciones (32 samples por radio)
            int samples = Mathf.Max(8, Mathf.CeilToInt(8 * r / step));
            for (int i = 0; i < samples; i++)
            {
                float angle = (i / (float)samples) * Mathf.PI * 2f;
                Vector2 candidate = origin + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;

                if (!IsOverlappingWalls(candidate))
                {
                    // Movemos al player a la posición libre encontrada
                    playerTransform.position = candidate;
                    Debug.Log($"LayoutManager: moví player a posición segura a distancia {r:F2}");
                    return;
                }
            }
        }

        // Si no encontramos posición libre, como último recurso lo movemos hacia arriba maxD
        playerTransform.position = origin + Vector2.up * maxD;
        Debug.LogWarning("LayoutManager: no encontré posición libre en rango, teletransporté al player arriba.");
    }

    private bool IsOverlappingWalls(Vector2 pos)
    {
        // Usamos el bounds del collider del player para hacer la comprobación exacta.
        // Si el collider es BoxCollider2D usamos su size y offset.
        // Para simplificar, usaremos OverlapBox con el tamaño del bounds mundial.

        Bounds bounds = playerCollider.bounds;
        Vector2 size = bounds.size;
        // OverlapBox requiere centro y size; rotación 0 (para rotaciones complejas habría que rotar).
        Collider2D hit = Physics2D.OverlapBox(pos, size, 0f, wallLayer);
        return hit != null;
    }

    // Visualización en editor
    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null && playerCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, maxResolveDistance);
            // dibujar el bounds actual del collider en la posición actual del player
            var b = playerCollider.bounds;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(playerTransform.position, b.size);
        }
    }
}
