using UnityEngine;
using System.Collections.Generic;

public class DeliveryProgression : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        public int requiredPoints;
        public GameObject deliveryZone;
        public GameObject materialPrefab;
    }

    public Stage[] stages;

    private List<GameObject> unlockedMaterials;
    private bool[] stageActivated;

    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        // Garantiza que nada empiece nulo
        unlockedMaterials = new List<GameObject>();
        stageActivated = new bool[stages.Length];
    }

    private void Start()
    {
        // Validación
        if (stages == null || stages.Length == 0)
        {
            Debug.LogError("DeliveryProgression: No hay stages configurados.");
            return;
        }

        // Apagar todas las delivery zones
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].deliveryZone != null)
                stages[i].deliveryZone.SetActive(false);
        }

        // Activar stage inicial
        TryActivateStages(0);

        IsInitialized = true;
    }

    public GameObject GetRandomUnlockedMaterial(int points)
    {
        if (!IsInitialized)
            return null;

        TryActivateStages(points);

        if (unlockedMaterials.Count == 0)
            return null;

        return unlockedMaterials[Random.Range(0, unlockedMaterials.Count)];
    }

    public void TryActivateStages(int points)
    {
        for (int i = 0; i < stages.Length; i++)
        {
            if (!stageActivated[i] && points >= stages[i].requiredPoints)
            {
                stageActivated[i] = true;

                if (stages[i].deliveryZone != null)
                    stages[i].deliveryZone.SetActive(true);

                if (stages[i].materialPrefab != null)
                    unlockedMaterials.Add(stages[i].materialPrefab);
            }
        }
    }
}
