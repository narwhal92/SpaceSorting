using UnityEngine;

public class AdaptiveBox : MonoBehaviour
{
    [Header("Камера")]
    public Camera cam;

    [Header("Части коробки")]
    public Transform floor;
    public Transform wallTop;
    public Transform wallBottom;
    public Transform wallLeft;
    public Transform wallRight;

    [Header("Люк и контейнеры")]
    public Transform spawnHatch;        // люк сверху
    public float hatchInset = 1f;       // отступ люка от верхнего края
    public Transform[] containers;      // контейнеры слева направо
    public float containersInset = 2f;  // отступ контейнеров от нижнего края
    public float containerGap = 0.5f;   // зазор между контейнерами

    [Header("Размеры (в юнитах)")]
    public float width = 16f;
    public float referenceWidth = 16f;  // при этой ширине размеры грузов и контейнеров "как нарисованы"
    public float wallThickness = 1.5f;
    public float wallHeight = 3f;
    public float sidePadding = 1.5f;
    public float topMargin = 3.5f;
    public float bottomMargin = 3.5f;

    [Header("Пределы длины коробки")]
    public float minDepth = 16f;
    public float maxDepth = 40f;

    public float Depth { get; private set; }

    // Во сколько раз масштабировать грузы, контейнеры и скорости
    public float Scale
    {
        get { return referenceWidth > 0f ? width / referenceWidth : 1f; }
    }

    int lastW, lastH;
    Vector3[] containerBaseScale;

    void Awake()
    {
        CaptureContainerScales();
        lastW = Screen.width;
        lastH = Screen.height;
        Apply();
    }

    void LateUpdate()
    {
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;
            Apply();
        }
    }

    void CaptureContainerScales()
    {
        if (containers == null) return;
        containerBaseScale = new Vector3[containers.Length];
        for (int i = 0; i < containers.Length; i++)
            if (containers[i]) containerBaseScale[i] = containers[i].localScale;
    }

    void Apply()
    {
        float aspect = (float)Screen.width / Screen.height;

        // Угол камеры берём из её реального поворота (90° = строго вниз)
        Vector3 f = cam.transform.forward;
        float sinT = Mathf.Clamp(-f.y, 0.2f, 1f);
        float cosT = Mathf.Sqrt(1f - sinT * sinT);
        float wallShift = wallHeight * cosT;

        // 1. Камера по ширине
        float size = (width + 2f * sidePadding) / 2f / aspect;

        // 2. Длина коробки, которая заполняет экран по высоте
        float depth = (2f * size - topMargin - bottomMargin - wallShift) / sinT;

        // 3. Слишком широкий экран: держим минимум, камеру отдаляем
        if (depth < minDepth)
        {
            depth = minDepth;
            size = (depth * sinT + wallShift + topMargin + bottomMargin) / 2f;
        }

        // 4. Слишком вытянутый экран: ограничиваем максимум
        depth = Mathf.Min(depth, maxDepth);
        Depth = depth;

        cam.orthographic = true;
        cam.orthographicSize = size;

        float vView = wallShift / 2f + (topMargin - bottomMargin) / 2f;
        Vector3 target = transform.position + new Vector3(0f, 0f, vView / sinT);
        cam.transform.position = target - f * 50f;

        // Детали коробки
        float t = wallThickness;
        Place(floor,      0f,                  0f,                  width, depth);
        Place(wallTop,    0f,                  depth / 2f - t / 2f, width, t);
        Place(wallBottom, 0f,                 -depth / 2f + t / 2f, width, t);
        Place(wallLeft,  -width / 2f + t / 2f, 0f,                  t,     depth - 2f * t);
        Place(wallRight,  width / 2f - t / 2f, 0f,                  t,     depth - 2f * t);

        // Люк
        if (spawnHatch) SetZ(spawnHatch, depth / 2f - hatchInset);

        // Контейнеры
        LayoutContainers(depth);
    }

    void LayoutContainers(float depth)
    {
        if (containers == null || containers.Length == 0) return;

        int n = containers.Length;
        if (containerBaseScale == null || containerBaseScale.Length != n)
            CaptureContainerScales();

        float inner = width - 2f * wallThickness;
        float slot = (inner - containerGap * (n - 1)) / n;
        float z = -depth / 2f + containersInset;

        for (int i = 0; i < n; i++)
        {
            Transform c = containers[i];
            if (!c) continue;

            float x = -inner / 2f + slot / 2f + i * (slot + containerGap);
            Vector3 p = c.localPosition;
            p.x = x;
            p.z = z;
            c.localPosition = p;

            c.localScale = containerBaseScale[i] * Scale;
        }
    }

    // Случайная точка внутри поля (для появления грузов)
    public Vector3 RandomPoint(float margin, float y)
    {
        float hx = width / 2f - wallThickness - margin;
        float hz = Depth / 2f - wallThickness - margin;
        Vector3 local = new Vector3(Random.Range(-hx, hx), 0f, Random.Range(-hz, hz));
        Vector3 p = transform.TransformPoint(local);
        p.y = y;
        return p;
    }

    void Place(Transform tr, float x, float z, float sx, float sz)
    {
        if (!tr) return;
        Vector3 p = tr.localPosition; p.x = x; p.z = z; tr.localPosition = p;
        Vector3 s = tr.localScale;    s.x = sx; s.z = sz; tr.localScale = s;
    }

    void SetZ(Transform tr, float z)
    {
        Vector3 p = tr.localPosition; p.z = z; tr.localPosition = p;
    }
}