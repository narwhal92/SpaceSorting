using UnityEngine;

public class CargoFloat : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 1.5f;

    [SerializeField] private float directionChangeInterval = 3f;
    [SerializeField] private float directionChangeStrength = 0.3f;

    [Header("Room Bounds")]
    [SerializeField] private float minX = -14f;
    [SerializeField] private float maxX = 14f;

    [SerializeField] private float minY = 0.5f;
    [SerializeField] private float maxY = 4f;

    [SerializeField] private float minZ = -12f;
    [SerializeField] private float maxZ = 12f;

    [Header("Rotation")]
    [SerializeField] private float minRotationSpeed = 10f;
    [SerializeField] private float maxRotationSpeed = 40f;

    private CargoDrag cargoDrag;

    private Vector3 velocity;

    private Vector3 rotationAxis;
    private float rotationSpeed;

    private float nextDirectionChangeTime;

    private void Start()
    {
        cargoDrag = GetComponent<CargoDrag>();

        velocity = Random.onUnitSphere *
                   Random.Range(minSpeed, maxSpeed);

        rotationAxis = Random.onUnitSphere;

        rotationSpeed = Random.Range(
            minRotationSpeed,
            maxRotationSpeed
        );

        nextDirectionChangeTime =
            Time.time + directionChangeInterval;
    }

    private void Update()
    {
        if (cargoDrag != null && cargoDrag.IsDragging)
            return;

        MoveCargo();
        RotateCargo();
        RandomlyAdjustDirection();
    }

    private void MoveCargo()
    {
        transform.position += velocity * Time.deltaTime;

        Vector3 pos = transform.position;

        if (pos.x < minX || pos.x > maxX)
        {
            velocity.x *= -1f;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
        }

        if (pos.y < minY || pos.y > maxY)
        {
            velocity.y *= -1f;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        if (pos.z < minZ || pos.z > maxZ)
        {
            velocity.z *= -1f;
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        }

        transform.position = pos;
    }

    private void RotateCargo()
    {
        transform.Rotate(
            rotationAxis,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }

    private void RandomlyAdjustDirection()
    {
        if (Time.time < nextDirectionChangeTime)
            return;

        velocity += Random.onUnitSphere *
                    directionChangeStrength;

        velocity = velocity.normalized *
                   Mathf.Clamp(
                       velocity.magnitude,
                       minSpeed,
                       maxSpeed
                   );

        nextDirectionChangeTime =
            Time.time +
            Random.Range(
                directionChangeInterval * 0.5f,
                directionChangeInterval * 1.5f
            );
    }
}