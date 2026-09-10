using UnityEngine;

public class CargoFloat : MonoBehaviour
{
    [SerializeField]
    private float horizontalAmplitude = 0.4f;

    [SerializeField]
    private float moveSpeed = 0.5f;

    private CargoDrag cargoDrag;

    private Vector3 startPosition;
    private Vector3 floatOffset;

    private Vector3 rotationAxis;
    private float rotationSpeed;

    private float personalSpeedMultiplier;
    private float personalVerticalAmplitude;

    private void Start()
    {
        cargoDrag = GetComponent<CargoDrag>();

        startPosition = transform.position;

        floatOffset = new Vector3(
            Random.Range(0f, 100f),
            Random.Range(0f, 100f),
            Random.Range(0f, 100f)
        );

        rotationAxis = Random.onUnitSphere;

        rotationSpeed = Random.Range(10f, 35f);

        personalSpeedMultiplier =
            Random.Range(0.8f, 1.3f);

        // Каждый груз получает свою высоту плавания
        personalVerticalAmplitude =
            Random.Range(0.8f, 2.2f);

        // Немного разбрасываем грузы по высоте уже при старте
        startPosition.y += Random.Range(-0.5f, 1.5f);
    }

    private void Update()
    {
        if (cargoDrag != null && cargoDrag.IsDragging)
            return;

        float time =
            Time.time *
            moveSpeed *
            personalSpeedMultiplier;

        float x =
            Mathf.Sin(time + floatOffset.x)
            * horizontalAmplitude;

        float z =
            Mathf.Cos(time + floatOffset.z)
            * horizontalAmplitude;

        float y =
            Mathf.Sin(time * 0.7f + floatOffset.y)
            * personalVerticalAmplitude;

        transform.position = new Vector3(
            startPosition.x + x,
            startPosition.y + y,
            startPosition.z + z
        );

        transform.Rotate(
            rotationAxis,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }
}