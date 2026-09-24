using UnityEngine;

public class CargoSpawner : MonoBehaviour
{
    [Header("Cargo Prefabs")]
    [SerializeField] private GameObject energyPrefab;
    [SerializeField] private GameObject bioPrefab;
    [SerializeField] private GameObject techPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int cargoCount = 12;

    [SerializeField] private float minX = -7f;
    [SerializeField] private float maxX = 7f;

    [SerializeField] private float minY = 1f;
    [SerializeField] private float maxY = 3f;

    [SerializeField] private float minZ = -10f;
    [SerializeField] private float maxZ = 10f;

    private void Start()
    {
        SpawnCargo();
    }

    private void SpawnCargo()
    {
        for (int i = 0; i < cargoCount; i++)
        {
            GameObject prefab = GetRandomCargo();

            Vector3 position = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                Random.Range(minZ, maxZ)
            );

            Instantiate(
                prefab,
                position,
                Random.rotation
            );
        }
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