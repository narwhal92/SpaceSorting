using UnityEngine;

public class CargoFloat : MonoBehaviour
{
    [SerializeField]
    private float moveAmplitude = 0.5f;

    [SerializeField]
    private float moveSpeed = 0.5f;

    [SerializeField]
    private float rotationSpeed = 20f;

    private CargoDrag cargoDrag;

    private Vector3 floatOffset;

    private void Start()
    {
        cargoDrag = GetComponent<CargoDrag>();

        floatOffset = new Vector3(
            Random.Range(0f, 100f),
            0f,
            Random.Range(0f, 100f)
        );
    }

    private void Update()
    {
        if (cargoDrag != null && cargoDrag.IsDragging)
            return;

        float x =
            Mathf.Sin(Time.time * moveSpeed + floatOffset.x)
            * moveAmplitude
            * Time.deltaTime;

        float z =
            Mathf.Cos(Time.time * moveSpeed + floatOffset.z)
            * moveAmplitude
            * Time.deltaTime;

        transform.position += new Vector3(x, 0, z);

        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }
}