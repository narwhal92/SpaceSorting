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

    [Header("Привязка к краям поля (необязательно)")]
    public Transform containersRow;    // ряд из 3 контейнеров
    public Transform spawnHatch;       // люк, откуда выплывают грузы
    public float containersInset = 2f; // насколько контейнеры отстоят от нижнего края
    public float hatchInset = 1f;      // насколько люк отстоит от верхнего края

    [Header("Размеры (в юнитах)")]
    public float width = 16f;
    public float wallThickness = 1.5f;
    public float wallHeight = 3f;
    public float sidePadding = 1.5f;
    public float topMargin = 3.5f;
    public float bottomMargin = 3.5f;

    [Header("Пределы длины коробки")]
    public float minDepth = 16f;
    public float maxDepth = 40f;

    public float Depth { get; private set; }

    int lastW, lastH;

    void LateUpdate()
    {
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;
            Apply();
        }
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

        // Камера смотрит в точку на полу с учётом наклона и отступов
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

        // Контейнеры и люк привязываем к краям поля
        if (containersRow) SetZ(containersRow, -depth / 2f + containersInset);
        if (spawnHatch)    SetZ(spawnHatch,     depth / 2f - hatchInset);
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