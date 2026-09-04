using UnityEngine;

public class CargoFloat : MonoBehaviour
{
    private Vector3 startPosition;

    [SerializeField]
    private float moveAmplitude = 0.5f;

    [SerializeField]
    private float moveSpeed = 0.5f;

    [SerializeField]
    private float rotationSpeed = 20f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float offsetX = Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
        float offsetZ = Mathf.Cos(Time.time * moveSpeed * 0.8f) * moveAmplitude;

        transform.position = new Vector3(
            startPosition.x + offsetX,
            transform.position.y,
            startPosition.z + offsetZ
        );

        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }
}