using UnityEngine;

public class CargoDrag : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging;

    [SerializeField]
    private float dragHeight = 1.2f;

    [SerializeField]
    private float minX = -15f;

    [SerializeField]
    private float maxX = 15f;

    [SerializeField]
    private float minZ = -13f;

    [SerializeField]
    private float maxZ = 13f;

    public bool IsDragging
    {
        get { return isDragging; }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        isDragging = true;

        Vector3 pos = transform.position;
        pos.y = dragHeight;
        transform.position = pos;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void Update()
    {
        if (!isDragging)
            return;

        Plane plane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);

            float clampedX = Mathf.Clamp(point.x, minX, maxX);
            float clampedZ = Mathf.Clamp(point.z, minZ, maxZ);

            transform.position = new Vector3(
                clampedX,
                dragHeight,
                clampedZ
            );
        }
    }
}