var mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>("Assets/Art/Prototype/BeveledBlock.asset");
System.Func<string,UnityEngine.Material> mat = path => UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(path);
var floorMat = mat("Assets/Art/CargoBay/Floor.mat");
var alternate = mat("Assets/Art/CargoBay/FloorAlternate.mat");
var gap = mat("Assets/Art/CargoBay/FloorGap.mat");
var marking = mat("Assets/Art/CargoBay/Marking.mat");
var shell = mat("Assets/Art/Prototype/Shell.mat");
var steel = mat("Assets/Art/Prototype/Steel.mat");
var panel = mat("Assets/Art/Prototype/Door.mat");
var lightMat = mat("Assets/Art/Prototype/ColdLight.mat");
var warm = mat("Assets/Art/Prototype/WarmAccent.mat");
System.Func<UnityEngine.Transform,string,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Material,UnityEngine.Transform> part = (parent,name,p,s,m) => {
    var g = new UnityEngine.GameObject(name);
    UnityEditor.Undo.RegisterCreatedObjectUndo(g,"Cargo bay art");
    g.transform.SetParent(parent,false);
    g.transform.localPosition = p;
    g.transform.localScale = s;
    g.AddComponent<UnityEngine.MeshFilter>().sharedMesh = mesh;
    var renderer = g.AddComponent<UnityEngine.MeshRenderer>();
    renderer.sharedMaterial = m;
    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    return g.transform;
};
var floor = UnityEngine.GameObject.Find("Floor").transform;
UnityEditor.Undo.RegisterFullObjectHierarchyUndo(floor.gameObject,"Panel floor");
floor.GetComponent<UnityEngine.Renderer>().sharedMaterial = gap;
var existing = floor.Find("FloorArt");
if (existing != null) UnityEditor.Undo.DestroyObjectImmediate(existing.gameObject);
var floorArt = new UnityEngine.GameObject("FloorArt").transform;
floorArt.SetParent(floor,false);
for(int x=0;x<4;x++) for(int z=0;z<8;z++) {
    var tile = part(floorArt,"DeckPanel_"+x+"_"+z,new UnityEngine.Vector3((x+.5f)/4f-.5f,.60f,(z+.5f)/8f-.5f),new UnityEngine.Vector3(.2475f,.1235f,.10f),(x+z)%3==0?alternate:floorMat);
    tile.GetComponent<UnityEngine.MeshFilter>().sharedMesh=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>("Assets/Art/CargoBay/DeckPanel.asset");
    tile.localRotation = UnityEngine.Quaternion.Euler(90,0,0);
}
for(int side=-1;side<=1;side+=2) for(int z=0;z<4;z++) {
    part(floorArt,"LaneGuide",new UnityEngine.Vector3(side*.402f,.69f,(z+.5f)/4f-.5f),new UnityEngine.Vector3(.003f,.025f,.17f),marking);
    if(z==0||z==3) part(floorArt,"ServiceMark",new UnityEngine.Vector3(side*.375f,.7f,(z+.5f)/4f-.5f),new UnityEngine.Vector3(.035f,.025f,.006f),warm);
}
foreach(var name in new[]{"WallLeft","WallRight","WallTop","WallBottom"}) {
    var wall=UnityEngine.GameObject.Find(name).transform;
    UnityEditor.Undo.RegisterFullObjectHierarchyUndo(wall.gameObject,"Sectioned wall");
    wall.GetComponent<UnityEngine.MeshFilter>().sharedMesh=mesh;
    wall.GetComponent<UnityEngine.Renderer>().sharedMaterial=shell;
    var old=wall.Find("WallArt");
    if(old!=null)UnityEditor.Undo.DestroyObjectImmediate(old.gameObject);
    var decor=new UnityEngine.GameObject("WallArt").transform;
    decor.SetParent(wall,false);
    bool alongZ=name=="WallLeft"||name=="WallRight";
    int count=alongZ?6:4;
    float inward=name=="WallLeft"||name=="WallBottom"?1f:-1f;
    for(int i=0;i<count;i++) {
        float u=(i+.5f)/count-.5f;
        part(decor,"WallPanel_"+i,alongZ?new UnityEngine.Vector3(inward*.51f,0,u):new UnityEngine.Vector3(u,0,inward*.51f),alongZ?new UnityEngine.Vector3(.10f,.78f,.145f):new UnityEngine.Vector3(.205f,.78f,.10f),panel);
        if(i%2==0) {
            part(decor,"EmbeddedLight_"+i,alongZ?new UnityEngine.Vector3(0,.51f,u):new UnityEngine.Vector3(u,.51f,0),alongZ?new UnityEngine.Vector3(.15f,.016f,.10f):new UnityEngine.Vector3(.14f,.016f,.15f),lightMat);
        }
    }
    for(int i=0;i<=count;i++) {
        float u=(float)i/count-.5f;
        var rib=part(decor,"Support_"+i,alongZ?new UnityEngine.Vector3(0,0,u):new UnityEngine.Vector3(u,0,0),alongZ?new UnityEngine.Vector3(1.02f,1.02f,.017f):new UnityEngine.Vector3(.025f,1.02f,1.02f),steel);
        rib.GetComponent<UnityEngine.Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.On;
    }
    if(!alongZ)for(int side=-1;side<=1;side+=2)
        part(decor,"CornerBracket",new UnityEngine.Vector3(side*.47f,0,0),new UnityEngine.Vector3(.06f,1.04f,1.03f),steel);
}
var sun=UnityEngine.GameObject.Find("Directional Light").GetComponent<UnityEngine.Light>();
UnityEditor.Undo.RecordObject(sun,"Cargo bay lighting");UnityEditor.Undo.RecordObject(sun.transform,"Cargo bay lighting");
sun.transform.rotation=UnityEngine.Quaternion.Euler(55,315,0);
sun.color=new UnityEngine.Color(.83f,.91f,1f);sun.intensity=1.4f;sun.shadowStrength=.65f;sun.shadowBias=.035f;sun.shadowNormalBias=.2f;
UnityEngine.RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;
UnityEngine.RenderSettings.ambientSkyColor=new UnityEngine.Color(.22f,.30f,.42f);
UnityEngine.RenderSettings.ambientEquatorColor=new UnityEngine.Color(.11f,.16f,.24f);
UnityEngine.RenderSettings.ambientGroundColor=new UnityEngine.Color(.035f,.055f,.085f);
var cam=UnityEngine.GameObject.Find("Main Camera").GetComponent<UnityEngine.Camera>();
UnityEditor.Undo.RecordObject(cam,"Cargo bay camera");
cam.clearFlags=UnityEngine.CameraClearFlags.SolidColor;cam.backgroundColor=new UnityEngine.Color(.012f,.022f,.038f);cam.allowHDR=true;
var data=cam.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
UnityEditor.Undo.RecordObject(data,"Camera post processing");data.renderPostProcessing=true;
var volume=UnityEngine.Object.FindFirstObjectByType<UnityEngine.Rendering.Volume>();
const string profilePath="Assets/Art/CargoBay/CargoBayProfile.asset";
if(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.VolumeProfile>(profilePath)==null)UnityEditor.AssetDatabase.CopyAsset(UnityEditor.AssetDatabase.GetAssetPath(volume.sharedProfile),profilePath);
var profile=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.VolumeProfile>(profilePath);
UnityEditor.Undo.RecordObject(volume,"Cargo bay profile");volume.sharedProfile=profile;
UnityEngine.Rendering.Universal.Bloom bloom;
if(!profile.TryGet<UnityEngine.Rendering.Universal.Bloom>(out bloom))bloom=profile.Add<UnityEngine.Rendering.Universal.Bloom>();
bloom.active=true;bloom.intensity.Override(.18f);bloom.threshold.Override(1.3f);bloom.scatter.Override(.45f);
UnityEngine.Rendering.Universal.MotionBlur blur;if(profile.TryGet<UnityEngine.Rendering.Universal.MotionBlur>(out blur))blur.active=false;
UnityEngine.Rendering.Universal.Vignette vignette;if(profile.TryGet<UnityEngine.Rendering.Universal.Vignette>(out vignette)){vignette.intensity.Override(.12f);vignette.smoothness.Override(.4f);}
foreach(var component in profile.components)UnityEditor.EditorUtility.SetDirty(component);
UnityEditor.EditorUtility.SetDirty(profile);
var hud=UnityEngine.GameObject.Find("OrderText").GetComponent<TMPro.TMP_Text>();
UnityEditor.Undo.RecordObject(hud,"Readable existing HUD");UnityEditor.Undo.RecordObject(hud.rectTransform,"HUD header position");
hud.fontSize=44;hud.enableAutoSizing=true;hud.fontSizeMin=28;hud.fontSizeMax=44;hud.rectTransform.pivot=new UnityEngine.Vector2(.5f,1);hud.rectTransform.anchorMin=new UnityEngine.Vector2(0,1);hud.rectTransform.anchorMax=new UnityEngine.Vector2(1,1);hud.rectTransform.anchoredPosition=new UnityEngine.Vector2(0,-48);hud.rectTransform.sizeDelta=new UnityEngine.Vector2(-80,200);
var scaler=hud.GetComponentInParent<UnityEngine.UI.CanvasScaler>();UnityEditor.Undo.RecordObject(scaler,"HUD scaling");scaler.matchWidthOrHeight=.5f;
UnityEditor.AssetDatabase.SaveAssets();UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
return new{sceneColliders=UnityEngine.Object.FindObjectsByType<UnityEngine.Collider>(UnityEngine.FindObjectsSortMode.None).Length,lights=UnityEngine.Object.FindObjectsByType<UnityEngine.Light>(UnityEngine.FindObjectsSortMode.None).Length,floorPanels=32};
