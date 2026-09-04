using UnityEngine;

public class CargoDrag : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (!isDragging)
            return;

        Plane plane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);

            transform.position = new Vector3(
                point.x,
                transform.position.y,
                point.z
            );
        }
    }
}