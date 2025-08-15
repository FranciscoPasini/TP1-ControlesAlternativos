using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefab & Pool")]
    public GameObject prefab;
    [SerializeField] private bool prewarm = true;

    [Header("Capacidad (ramp opcional)")]
    [SerializeField] private int maxCapacity = 6;   // capacidad final
    [SerializeField] private int startCapacity = 1; // capacidad inicial
    [SerializeField] private bool rampCapacity = true;
    [SerializeField, Min(0.1f)] private float capacityRampDuration = 90f; // seg a full
    [SerializeField, Min(1f)] private float capacityEasePower = 2f; // >1 = más suave al inicio

    [Header("Timing (ramp suave)")]
    [SerializeField, Min(0f)] private float firstSpawnDelay = 1.0f; // primer enemigo
    [SerializeField, Min(0.05f)] private float maxInterval = 6f;     // al inicio (lento)
    [SerializeField, Min(0.05f)] private float minInterval = 1.2f;   // al final (rápido)
    [SerializeField, Min(0.1f)] private float rampDuration = 120f;   // seg hasta llegar a minInterval
    [SerializeField, Min(1f)] private float easePower = 2.5f;      // >1 = acelera de a poco

    [Header("Variación aleatoria del intervalo")]
    [SerializeField, Range(0f, 1f)] private float jitterStart = 0.03f; // al comienzo (casi constante)
    [SerializeField, Range(0f, 1f)] private float jitterEnd = 0.20f; // al final (más caótico)

    [Header("Posición (opcional)")]
    [SerializeField] private float spawnRadius = 0f;         // 0 = exactamente en el Spawner
    [SerializeField] private bool randomInsideCircle = true; // true = dentro del círculo, false = borde

    // -----------------------------------------

    private readonly List<GameObject> pool = new();
    private float nextSpawnAt;
    private float startTime;

    void Awake()
    {
        // Coherencia básica
        if (minInterval > maxInterval) minInterval = maxInterval;
        startCapacity = Mathf.Clamp(startCapacity, 0, Mathf.Max(1, maxCapacity));
    }

    void Start()
    {
        if (prewarm) Prewarm(rampCapacity ? startCapacity : maxCapacity);

        startTime = Time.time;
        nextSpawnAt = Time.time + firstSpawnDelay; // primer spawn independiente del intervalo
    }

    void Update()
    {
        if (Time.time >= nextSpawnAt)
        {
            TrySpawn();
            ScheduleNext();
        }
    }

    // ============== RAMP LÓGICO ==============

    // Ease-in con potencia: t^easePower (t 0..1)
    float Ease01(float t, float power) => Mathf.Pow(Mathf.Clamp01(t), power);

    float CurrentInterval()
    {
        float t = (Time.time - startTime) / rampDuration; // 0..1 en rampDuration
        float eased = Ease01(t, easePower);               // suave al principio
        return Mathf.Lerp(maxInterval, minInterval, eased);
    }

    float CurrentJitter()
    {
        float t = Mathf.Clamp01((Time.time - startTime) / rampDuration);
        return Mathf.Lerp(jitterStart, jitterEnd, t);
    }

    int CurrentCapacity()
    {
        if (!rampCapacity) return maxCapacity;

        float t = (Time.time - startTime) / capacityRampDuration;
        float eased = Ease01(t, capacityEasePower);
        return Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(startCapacity, maxCapacity, eased)), 0, maxCapacity);
    }

    void ScheduleNext()
    {
        float baseInterval = CurrentInterval();
        float jitter = 1f + Random.Range(-CurrentJitter(), CurrentJitter());
        nextSpawnAt = Time.time + Mathf.Max(0.05f, baseInterval * jitter);
    }

    // ============== POOL & SPAWN ==============

    void Prewarm(int capacity)
    {
        for (int i = pool.Count; i < capacity; i++)
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    int ActiveCount()
    {
        int c = 0;
        for (int i = 0; i < pool.Count; i++)
            if (pool[i] != null && pool[i].activeInHierarchy) c++;
        return c;
    }

    void TrySpawn()
    {
        // Limita por capacidad rampada
        if (ActiveCount() >= CurrentCapacity()) return;

        Spawn();
    }

    void Spawn()
    {
        GameObject obj = null;

        // Reusar del pool
        for (int i = 0; i < pool.Count; i++)
        {
            var p = pool[i];
            if (p != null && !p.activeInHierarchy) { obj = p; break; }
        }

        // Crear si falta y aún hay margen hasta el máximo (final)
        if (obj == null && pool.Count < maxCapacity)
        {
            obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }

        if (obj == null) return;

        Vector3 pos = transform.position;
        if (spawnRadius > 0f)
        {
            Vector2 off = randomInsideCircle ? Random.insideUnitCircle : Random.insideUnitCircle.normalized;
            pos += (Vector3)(off * spawnRadius);
        }

        obj.transform.SetPositionAndRotation(pos, transform.rotation);
        obj.SetActive(true);
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj != null) obj.SetActive(false);
    }
}
