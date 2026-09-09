using UnityEngine;

public class CargoRepulsion : MonoBehaviour
{
    [SerializeField]
    private float radius = 2f;

    [SerializeField]
    private float force = 2f;

    private CargoDrag cargoDrag;

    private void Start()
    {
        cargoDrag = GetComponent<CargoDrag>();
    }

    private void Update()
    {
        if (cargoDrag != null && cargoDrag.IsDragging)
            return;

        CargoRepulsion[] cargos =
            FindObjectsByType<CargoRepulsion>(
                FindObjectsSortMode.None
            );

        foreach (CargoRepulsion other in cargos)
        {
            if (other == this)
                continue;

            Vector3 direction =
                transform.position - other.transform.position;

            float distance = direction.magnitude;

            if (distance > radius)
                continue;

            if (distance < 0.01f)
                continue;

            float strength =
                (radius - distance) / radius;

            transform.position +=
                direction.normalized *
                strength *
                force *
                Time.deltaTime;
        }
    }
}