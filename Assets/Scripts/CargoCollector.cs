using System.Collections;
using UnityEngine;

public class CargoCollector : MonoBehaviour
{
    [SerializeField, Min(0.001f)] private float intakeDuration = 0.3f;
    private bool collecting;
    public bool IsCollecting => collecting;

    // Called only by an actual held-to-released transition, not by trigger entry.
    public bool TryRelease()
    {
        CargoDrag drag = GetComponent<CargoDrag>();
        if (collecting || drag == null || !drag.IsDragging) return false;
        LevelManager level = LevelManager.Instance;
        if (level == null || !level.CanDeliver(this)) return false;
        Physics.SyncTransforms();
        foreach (ContainerFeedback hatch in FindObjectsByType<ContainerFeedback>(FindObjectsSortMode.None))
        {
            if (hatch.isActiveAndEnabled && hatch.Matches(this) && hatch.Contains(transform.position))
            {
                BeginIntake(hatch, drag, level);
                return true;
            }
        }
        return false;
    }

    private void BeginIntake(ContainerFeedback hatch, CargoDrag drag, LevelManager level)
    {
        collecting = true;
        if (drag != null) drag.BeginCollection();
        CargoFloat drift = GetComponent<CargoFloat>();
        if (drift != null) drift.enabled = false;
        CargoRepulsion repulsion = GetComponent<CargoRepulsion>();
        if (repulsion != null) repulsion.enabled = false;
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null) body.isKinematic = true;
        foreach (Collider cargoCollider in GetComponentsInChildren<Collider>())
            cargoCollider.enabled = false;
        hatch.BeginIntake();
        StartCoroutine(Intake(hatch, drag.Visual, level));
    }

    private IEnumerator Intake(ContainerFeedback hatch, Transform visual, LevelManager level)
    {
        // Keep a valid quick release queued until the aperture is fully open.
        while (hatch != null && !hatch.IsFullyOpen) yield return null;
        Collider intake = hatch != null ? hatch.Intake : null;
        if (intake == null) yield break;
        Vector3 start = transform.position;
        Vector3 startScale = visual != null ? visual.localScale : Vector3.one;
        float elapsed = 0f;
        float duration = Mathf.Max(0.001f, intakeDuration);
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            if (t < 0.35f)
            {
                // Approach the visible aperture at full size, never fade in front of it.
                transform.position = Vector3.Lerp(start, hatch.ApproachPosition, Mathf.SmoothStep(0f, 1f, t / 0.35f));
            }
            else
            {
                float insideT = Mathf.SmoothStep(0f, 1f, (t - 0.35f) / 0.65f);
                transform.position = Vector3.Lerp(hatch.ApproachPosition, hatch.RecessPosition, insideT);
            }
            Vector3 depth = hatch.RecessPosition - hatch.MouthPosition;
            float entered = depth.sqrMagnitude > 0.0001f
                ? Mathf.Clamp01(Vector3.Dot(transform.position - hatch.MouthPosition, depth) / depth.sqrMagnitude)
                : 0f;
            if (visual != null) visual.localScale = startScale * (1f - Mathf.SmoothStep(0f, 1f, entered));
            yield return null;
            elapsed += Time.deltaTime;
        }
        transform.position = hatch.RecessPosition;
        if (visual != null) visual.localScale = Vector3.zero;
        if (level != null && level.TryDeliver(this)) Debug.Log("Correct!");
        if (hatch != null) hatch.EndIntake();
        Destroy(gameObject);
    }
}
