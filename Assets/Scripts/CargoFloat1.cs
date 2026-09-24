using UnityEngine;

public class CargoFloat1 : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minSpeed = 1.0f;
    [SerializeField] private float maxSpeed = 2.0f;

    [SerializeField] private float directionChangeInterval = 2f;
    [SerializeField] private float directionChangeStrength = 0.4f;

    [Header("Room Bounds")]
    [SerializeField] private float minX = -7f;
    [SerializeField] private float maxX = 7f;

    [SerializeField] private float minY = 0.5f;
    [SerializeField] private float maxY = 4f;

    [SerializeField] private float minZ = -10f;
    [SerializeField] private float maxZ = 10f;

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

        GenerateNewVelocity();

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

    private void GenerateNewVelocity()
    {
        float speed = Random.Range(minSpeed, maxSpeed);

        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);

        float y = Random.Range(-0.25f, 0.25f);

        velocity = new Vector3(x, y, z).normalized * speed;
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

        Vector3 drift = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.2f, 0.2f),
            Random.Range(-1f, 1f)
        );

        velocity += drift * directionChangeStrength;

        velocity = velocity.normalized *
                   Random.Range(minSpeed, maxSpeed);

        nextDirectionChangeTime =
            Time.time +
            Random.Range(
                directionChangeInterval * 0.5f,
                directionChangeInterval * 1.5f
            );
    }
}