var rootPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(UnityEngine.Application.dataPath, "..", "Docs", "Checks", "2026-10-20-cargo-bay"));
var framesPath = System.IO.Path.Combine(rootPath, "frames");
System.IO.Directory.CreateDirectory(framesPath);
var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
var grab = typeof(CargoDrag).GetMethod("OnMouseDown", flags);
var release = typeof(CargoDrag).GetMethod("OnMouseUp", flags);
var all = UnityEngine.Object.FindObjectsByType<CargoCollector>(UnityEngine.FindObjectsSortMode.None);
var bio = all.GroupBy(c => c.tag).First(g => g.Count() >= 2).Take(2).ToArray();
var tech = all.First(c => c.tag != bio[0].tag);
var energy = all.First(c => c.tag != bio[0].tag && c.tag != tech.tag);
var selected = new[] { bio[0], bio[1], tech, energy };
var grabs = new[] { 0.4f, 1.55f, 2.2f, 3.5f };
var moves = new[] { 1.0f, 1.55f, 2.6f, 3.9f };
var releases = new[] { 1.5f, 1.65f, 3.0f, 4.3f };
var started = new bool[4];
var ended = new bool[4];
var sources = new UnityEngine.Vector3[4];
var targets = new UnityEngine.Vector3[4];
for (int i = 0; i < selected.Length; i++)
{
    var c = selected[i];
    var d = c.GetComponent<CargoDrag>();
    typeof(CargoDrag).GetField("mainCamera", flags).SetValue(d, null);
    var hatch = UnityEngine.GameObject.Find(c.tag + "Container").GetComponent<ContainerFeedback>();
    targets[i] = hatch.transform.position + new UnityEngine.Vector3(0f, 3.7f, -0.2f);
    sources[i] = targets[i] + UnityEngine.Vector3.forward * 4.2f;
    c.transform.position = sources[i];
    c.transform.rotation = UnityEngine.Quaternion.Euler(20f, 30f, 10f);
}
var cam = UnityEngine.Camera.main;
var rt = new UnityEngine.RenderTexture(540, 960, 24);
rt.Create();
var tex = new UnityEngine.Texture2D(540, 960, UnityEngine.TextureFormat.RGB24, false);
int previousCapture = UnityEngine.Time.captureFramerate;
UnityEngine.Time.captureFramerate = 30;
UnityEngine.Time.timeScale = 1f;
UnityEngine.Application.runInBackground = true;
var intakeScales = new System.Collections.Generic.Dictionary<CargoCollector, UnityEngine.Vector3>();
var previousDoor = new System.Collections.Generic.Dictionary<ContainerFeedback, float>();
bool noShrinkOutside = true;
bool noEarlyClose = true;
bool overlapSeen = false;
int frame = 0;
int lastFrame = -1;
float startTime = UnityEngine.Time.time;
var timestamps = new System.Collections.Generic.List<string>();
UnityEditor.SessionState.SetString("CargoBayDemo.Status", "recording");
UnityEditor.EditorApplication.CallbackFunction update = null;
update = () =>
{
    if (!UnityEditor.EditorApplication.isPlaying || frame >= 150)
    {
        UnityEditor.EditorApplication.update -= update;
        UnityEngine.Time.captureFramerate = previousCapture;
        rt.Release();
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(tex);
        System.IO.File.WriteAllLines(System.IO.Path.Combine(rootPath, "frame-times.csv"), timestamps);
        System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "demo-validation.txt"),
            "noShrinkOutside=" + noShrinkOutside + "\nnoEarlyClose=" + noEarlyClose +
            "\noverlapSeen=" + overlapSeen + "\nselectedAllRemoved=" + selected.All(c => c == null) +
            "\nprogress=" + LevelManager.Instance.DeliveredCargo + "/" + LevelManager.Instance.TotalCargo);
        UnityEditor.SessionState.SetString("CargoBayDemo.Status", "completed:" + frame);
        return;
    }
    if (UnityEngine.Time.frameCount == lastFrame) { UnityEditor.EditorApplication.QueuePlayerLoopUpdate(); return; }
    lastFrame = UnityEngine.Time.frameCount;
    float t = frame / 30f;
    for (int i = 0; i < selected.Length; i++)
    {
        var c = selected[i];
        if (c == null) continue;
        if (!started[i] && t >= grabs[i]) { grab.Invoke(c.GetComponent<CargoDrag>(), null); started[i] = true; }
        if (started[i] && !ended[i])
        {
            float u = UnityEngine.Mathf.Clamp01((t - moves[i]) / UnityEngine.Mathf.Max(0.01f, releases[i] - moves[i]));
            c.transform.position = UnityEngine.Vector3.Lerp(sources[i], targets[i], u);
            if (t >= releases[i]) { release.Invoke(c.GetComponent<CargoDrag>(), null); ended[i] = true; if (c.IsCollecting) intakeScales[c] = c.GetComponent<CargoDrag>().Visual.localScale; }
        }
    }
    foreach (var c in selected)
    {
        if (c == null || !c.IsCollecting || !intakeScales.ContainsKey(c)) continue;
        var h = UnityEngine.GameObject.Find(c.tag + "Container").GetComponent<ContainerFeedback>();
        var depth = h.RecessPosition - h.MouthPosition;
        if (UnityEngine.Vector3.Dot(c.transform.position - h.MouthPosition, depth) <= 0f)
            noShrinkOutside &= (c.GetComponent<CargoDrag>().Visual.localScale - intakeScales[c]).sqrMagnitude < 0.0001f;
    }
    foreach (var h in UnityEngine.Object.FindObjectsByType<ContainerFeedback>(UnityEngine.FindObjectsSortMode.None))
    {
        float open = (float)typeof(ContainerFeedback).GetField("openness", flags).GetValue(h);
        if (h.PendingIntakes > 0 && previousDoor.ContainsKey(h)) noEarlyClose &= open + 0.0001f >= previousDoor[h];
        if (h.PendingIntakes >= 2) overlapSeen = true;
        previousDoor[h] = open;
    }
    var oldTarget = cam.targetTexture;
    var oldActive = UnityEngine.RenderTexture.active;
    try
    {
        cam.targetTexture = rt;
        cam.Render();
        UnityEngine.RenderTexture.active = rt;
        tex.ReadPixels(new UnityEngine.Rect(0, 0, 540, 960), 0, 0);
        tex.Apply();
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(framesPath, frame.ToString("D4") + ".png"), tex.EncodeToPNG());
    }
    finally { cam.targetTexture = oldTarget; UnityEngine.RenderTexture.active = oldActive; }
    timestamps.Add(frame + "," + (UnityEngine.Time.time - startTime).ToString("F5", System.Globalization.CultureInfo.InvariantCulture));
    frame++;
    UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
};
UnityEditor.EditorApplication.update += update;
UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
return "Recording 150 game-camera frames at 30 fps; scripted release, no mouse automation.";




