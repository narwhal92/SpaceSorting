using UnityEngine;

public class CargoDrag : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging;
    private AdaptiveBox box;
    private float restingHeight;
    [SerializeField] private Transform visual;
    [SerializeField, Range(0f, 0.25f)] private float jellyStrength = 0.20f;
    [SerializeField, Min(0.001f)] private float jellyDuration = 0.3f;
    [SerializeField, Min(0.001f)] private float releaseDuration = 0.1f;
    private Vector3 baseVisualScale;
    private Vector3 releaseScale;
    private float animationElapsed;
    private bool jellyPlaying;
    private bool returning;
    private bool collecting;
    public static CargoDrag ActiveDrag { get; private set; }
    public Transform Visual => visual;

    private void Awake()
    {
        if (visual != null) baseVisualScale = visual.localScale;
    }

    private void AnimateScale(bool held)
    {
        if (visual == null) return;
        animationElapsed = 0f;
        jellyPlaying = held;
        returning = !held;
        releaseScale = visual.localScale;
        if (held) visual.localScale = baseVisualScale;
    }

    private void UpdateVisual()
    {
        if (visual == null) return;
        animationElapsed += Time.deltaTime;
        if (jellyPlaying)
        {
            float t = Mathf.Clamp01(animationElapsed / Mathf.Max(0.001f, jellyDuration));
            float offset = JellyOffset(t) * jellyStrength;
            visual.localScale = Vector3.Scale(baseVisualScale, new Vector3(1f - offset, 1f + offset, 1f - offset));
            if (t >= 1f) { visual.localScale = baseVisualScale; jellyPlaying = false; }
        }
        else if (returning)
        {
            float t = Mathf.Clamp01(animationElapsed / Mathf.Max(0.001f, releaseDuration));
            visual.localScale = Vector3.Lerp(releaseScale, baseVisualScale, Mathf.SmoothStep(0f, 1f, t));
            if (t >= 1f) { visual.localScale = baseVisualScale; returning = false; }
        }
    }

    private static float JellyOffset(float t)
    {
        if (t < 0.18f) return Mathf.Lerp(0f, -1f, Mathf.SmoothStep(0f, 1f, t / 0.18f));
        if (t < 0.47f) return Mathf.Lerp(-1f, 0.72f, Mathf.SmoothStep(0f, 1f, (t - 0.18f) / 0.29f));
        if (t < 0.73f) return Mathf.Lerp(0.72f, -0.28f, Mathf.SmoothStep(0f, 1f, (t - 0.47f) / 0.26f));
        return Mathf.Lerp(-0.28f, 0f, Mathf.SmoothStep(0f, 1f, (t - 0.73f) / 0.27f));
    }

    private void OnDisable()
    {
        if (ActiveDrag == this) ActiveDrag = null;
        isDragging = false;
    }

    public void BeginCollection()
    {
        collecting = true;
        isDragging = false;
        enabled = false;
    }

    [SerializeField]
    private float dragHeight = 4.3f;

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
        box = FindFirstObjectByType<AdaptiveBox>();
        restingHeight = transform.position.y;
    }

    private void OnMouseDown()
    {
        if (collecting || !enabled) return;
        isDragging = true;
        ActiveDrag = this;
        AnimateScale(true);

        Vector3 pos = transform.position;
        pos.y = dragHeight;
        transform.position = pos;
    }

    private void OnMouseUp()
    {
        if (collecting || !isDragging) return;
        MoveToPointer();
        CargoCollector collector = GetComponent<CargoCollector>();
        if (collector != null && collector.TryRelease()) return;
        if (ActiveDrag == this) ActiveDrag = null;
        isDragging = false;
        AnimateScale(false);
        Vector3 pos = transform.position;
        pos.y = restingHeight;
        transform.position = pos;
    }

    private void Update()
    {
        UpdateVisual();
        if (!isDragging)
            return;

        MoveToPointer();
    }

    private void MoveToPointer()
    {
        if (mainCamera == null) return;
        Plane plane = new Plane(Vector3.up, new Vector3(0f, dragHeight, 0f));

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);

            float clampedX = Mathf.Clamp(point.x, minX, maxX);
            float clampedZ = Mathf.Clamp(point.z, minZ, maxZ);
            if (box != null && box.Depth > 0f)
            {
                float margin = 0.8f;
                Collider cargoCollider = GetComponent<Collider>();
                if (cargoCollider != null)
                    margin = Mathf.Max(margin, cargoCollider.bounds.extents.x + 0.1f,
                        cargoCollider.bounds.extents.z + 0.1f);
                Vector3 center = box.transform.position;
                float hx = box.width / 2f - box.wallThickness - margin;
                float hz = box.Depth / 2f - box.wallThickness - margin;
                clampedX = Mathf.Clamp(point.x, center.x - hx, center.x + hx);
                clampedZ = Mathf.Clamp(point.z, center.z - hz, center.z + hz);
            }

            transform.position = new Vector3(
                clampedX,
                dragHeight,
                clampedZ
            );
        }
    }
}
