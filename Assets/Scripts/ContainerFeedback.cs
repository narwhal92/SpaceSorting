using UnityEngine;

// Visual hatch only. The intake collider and AdaptiveBox root never animate.
public class ContainerFeedback : MonoBehaviour
{
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private Collider intake;
    [SerializeField] private Transform approachPoint;
    [SerializeField] private Transform mouthPoint;
    [SerializeField] private Transform recessPoint;
    public Vector3 ApproachPosition => approachPoint != null ? approachPoint.position : intake.bounds.center + Vector3.up;
    public Vector3 MouthPosition => mouthPoint != null ? mouthPoint.position : intake.bounds.center;
    public Vector3 RecessPosition => recessPoint != null ? recessPoint.position : intake.bounds.center;
    [SerializeField, Min(0f)] private float openingDistance = 2f;
    [SerializeField, Min(0.01f)] private float closingHysteresis = 0.6f;
    [SerializeField, Min(0.001f)] private float doorDuration = 0.18f;
    [SerializeField, Min(0f)] private float doorTravel = 0.36f;
    private Vector3 leftClosed;
    private Vector3 rightClosed;
    private float openness;
    private bool near;
    private int pendingIntakes;
    public bool IsFullyOpen => openness >= 1f;
    public int PendingIntakes => pendingIntakes;
    public Collider Intake => intake;

    private void Awake()
    {
        if (intake == null) intake = GetComponent<Collider>();
        if (leftDoor != null) leftClosed = leftDoor.localPosition;
        if (rightDoor != null) rightClosed = rightDoor.localPosition;
    }

    public bool Matches(CargoCollector cargo)
    {
        return cargo != null && tag == cargo.tag + "Container";
    }

    public bool Contains(Vector3 point)
    {
        if (intake == null || !intake.enabled) return false;
        return FootprintDistance(point) < 0.01f;
    }

    // Holding is above the housing; the existing X/Z intake footprint stays unchanged.
    private float FootprintDistance(Vector3 point)
    {
        point.y = intake.bounds.center.y;
        return Vector3.Distance(point, intake.ClosestPoint(point));
    }

    public void BeginIntake()
    {
        pendingIntakes++;
    }

    public void EndIntake()
    {
        pendingIntakes = Mathf.Max(0, pendingIntakes - 1);
    }

    private void Update()
    {
        CargoDrag held = CargoDrag.ActiveDrag;
        CargoCollector cargo = held != null ? held.GetComponent<CargoCollector>() : null;
        if (held != null && held.IsDragging && Matches(cargo) && intake != null)
        {
            float distance = FootprintDistance(held.transform.position);
            float threshold = openingDistance + (near ? closingHysteresis : 0f);
            near = distance <= threshold;
        }
        else near = false;

        float target = near || pendingIntakes > 0 ? 1f : 0f;
        openness = Mathf.MoveTowards(openness, target, Time.deltaTime / Mathf.Max(0.001f, doorDuration));
        float travel = Mathf.SmoothStep(0f, 1f, openness) * doorTravel;
        if (leftDoor != null) leftDoor.localPosition = leftClosed + Vector3.left * travel;
        if (rightDoor != null) rightDoor.localPosition = rightClosed + Vector3.right * travel;
    }

    private void OnDisable()
    {
        if (leftDoor != null) leftDoor.localPosition = leftClosed;
        if (rightDoor != null) rightDoor.localPosition = rightClosed;
        openness = 0f;
        near = false;
    }
}
