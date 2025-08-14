using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnInterval = 10f; // tiempo entre spawns
    private int maxCapacity = 3;
    private List<GameObject> pool = new List<GameObject>();
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        // Spawnear automáticamente cada cierto tiempo
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        // Buscar un objeto inactivo y reutilizarlo
        foreach (var obj in pool)
        {
            if (obj != null && !obj.activeInHierarchy)
            {
                obj.transform.position = transform.position;
                obj.SetActive(true);
                return;
            }
        }

        // Instanciar uno nuevo si no hemos alcanzado la capacidad máxima
        if (pool.Count < maxCapacity)
        {
            GameObject newObj = Instantiate(prefab, transform.position, transform.rotation);
            pool.Add(newObj);
        }
        else
        {
            Debug.Log("Capacidad máxima alcanzada");
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(false);
    }
}
