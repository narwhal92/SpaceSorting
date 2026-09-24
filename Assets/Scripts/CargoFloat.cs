using UnityEngine;

public class CargoFloat : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 1.0f;

    [SerializeField] private float directionChangeInterval = 2f;
    [SerializeField] private float directionChangeStrength = 0.4f;

    [Header("Room Bounds")]
    [SerializeField] private float margin = 0.8f; // примерно половина размера груза

    [Header("Rotation")]
    [SerializeField] private float minRotationSpeed = 10f;
    [SerializeField] private float maxRotationSpeed = 40f;

    private CargoDrag cargoDrag;
    private AdaptiveBox box;

    private Vector3 velocity;
    private Vector3 rotationAxis;
    private float rotationSpeed;
    private float nextDirectionChangeTime;
    private float fixedY;

    private void Start()
    {
        cargoDrag = GetComponent<CargoDrag>();
        box = FindObjectOfType<AdaptiveBox>();

        fixedY = transform.position.y;

        GenerateNewVelocity();

        rotationAxis = Random.onUnitSphere;
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

        nextDirectionChangeTime = Time.time + directionChangeInterval;
    }

    private void Update()
    {
        if (box == null || box.Depth <= 0f)
            return;

        if (cargoDrag != null && cargoDrag.IsDragging)
            return;

        MoveCargo();
        RotateCargo();
        RandomlyAdjustDirection();
    }

    private void LateUpdate()
    {
        // Страховка: что бы ни двигало груз (в том числе палец),
        // он остаётся внутри коробки и на своей высоте.
        if (box == null || box.Depth <= 0f)
            return;

        transform.position = ClampToBox(transform.position);
    }

    private void GenerateNewVelocity()
    {
        float speed = Random.Range(minSpeed, maxSpeed);

        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);

        velocity = new Vector3(x, 0f, z).normalized * speed;
    }

    private void GetHalfSize(out float hx, out float hz)
    {
        hx = box.width / 2f - box.wallThickness - margin;
        hz = box.Depth / 2f - box.wallThickness - margin;
    }

    private Vector3 ClampToBox(Vector3 pos)
    {
        Vector3 c = box.transform.position;
        GetHalfSize(out float hx, out float hz);

        pos.x = Mathf.Clamp(pos.x, c.x - hx, c.x + hx);
        pos.z = Mathf.Clamp(pos.z, c.z - hz, c.z + hz);
        pos.y = fixedY;
        return pos;
    }

    private void MoveCargo()
    {
        Vector3 c = box.transform.position;
        GetHalfSize(out float hx, out float hz);

        Vector3 pos = transform.position + velocity * Time.deltaTime;

        float dx = pos.x - c.x;
        float dz = pos.z - c.z;

        // Отскок: скорость всегда направляем от стены внутрь
        if (dx > hx)       { pos.x = c.x + hx; velocity.x = -Mathf.Abs(velocity.x); }
        else if (dx < -hx) { pos.x = c.x - hx; velocity.x =  Mathf.Abs(velocity.x); }

        if (dz > hz)       { pos.z = c.z + hz; velocity.z = -Mathf.Abs(velocity.z); }
        else if (dz < -hz) { pos.z = c.z - hz; velocity.z =  Mathf.Abs(velocity.z); }

        pos.y = fixedY;
        transform.position = pos;
    }

    private void RotateCargo()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void RandomlyAdjustDirection()
    {
        if (Time.time < nextDirectionChangeTime)
            return;

        Vector3 drift = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        );

        velocity += drift * directionChangeStrength;
        velocity.y = 0f;

        if (velocity.sqrMagnitude < 0.0001f)
            velocity = new Vector3(1f, 0f, 0f);

        velocity = velocity.normalized * Random.Range(minSpeed, maxSpeed);

        nextDirectionChangeTime = Time.time + Random.Range(
            directionChangeInterval * 0.5f,
            directionChangeInterval * 1.5f
        );
    }
}