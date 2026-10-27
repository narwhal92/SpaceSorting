using System.Collections.Generic;
using UnityEngine;

public class CargoSpawner : MonoBehaviour
{
    [Header("Cargo Prefabs")]
    [SerializeField] private GameObject energyPrefab;
    [SerializeField] private GameObject bioPrefab;
    [SerializeField] private GameObject techPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int cargoCount = 12;
    [SerializeField] private float cargoY = 1.5f;        // высота, на которой плавают все грузы
    [SerializeField] private float edgeMargin = 0.8f;    // отступ от стен (как margin у CargoFloat)
    [SerializeField] private float minDistance = 2.2f;   // минимальное расстояние между грузами при появлении

    private void Start()
    {
        SpawnCargo();
    }

    private void SpawnCargo()
    {
        AdaptiveBox box = FindObjectOfType<AdaptiveBox>();
        if (box == null)
        {
            Debug.LogWarning("CargoSpawner: на сцене нет AdaptiveBox");
            return;
        }

        float margin = edgeMargin * box.Scale;
        float minDist = minDistance * box.Scale;
        List<Vector3> placed = new List<Vector3>();
        List<CargoCollector> spawned = new List<CargoCollector>();

        for (int i = 0; i < cargoCount; i++)
        {
            GameObject prefab = GetRandomCargo();

            // Несколько попыток найти место подальше от уже стоящих грузов
            Vector3 position = box.RandomPoint(margin, cargoY);
            for (int attempt = 0; attempt < 30; attempt++)
            {
                position = box.RandomPoint(margin, cargoY);
                if (IsFarEnough(position, placed, minDist) && IsOutsideContainers(position, box))
                    break;
            }

            if (!IsFarEnough(position, placed, minDist) || !IsOutsideContainers(position, box)) continue;
            placed.Add(position);
            if (prefab == null) continue;
            GameObject instance = Instantiate(prefab, position, Random.rotation);
            CargoCollector collector = instance.GetComponent<CargoCollector>();
            if (collector != null) spawned.Add(collector);
        }

        LevelManager.Instance.PrepareLevel(spawned);
    }

    private bool IsOutsideContainers(Vector3 position, AdaptiveBox box)
    {
        // Exclude the same X/Z footprint used by release, regardless of hover height.
        float clearance = Mathf.Max(1f, edgeMargin) * box.Scale;
        foreach (Transform container in box.containers)
        {
            if (container == null) continue;
            Collider intake = container.GetComponent<Collider>();
            if (intake != null)
            {
                Vector3 footprintPoint = position;
                footprintPoint.y = intake.bounds.center.y;
                if ((intake.ClosestPoint(footprintPoint) - footprintPoint).sqrMagnitude < clearance * clearance)
                    return false;
            }
        }
        return true;
    }

    private bool IsFarEnough(Vector3 p, List<Vector3> others, float minDist)
    {
        for (int i = 0; i < others.Count; i++)
        {
            if ((others[i] - p).sqrMagnitude < minDist * minDist)
                return false;
        }
        return true;
    }

    private GameObject GetRandomCargo()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                return energyPrefab;

            case 1:
                return bioPrefab;

            default:
                return techPrefab;
        }
    }
}
