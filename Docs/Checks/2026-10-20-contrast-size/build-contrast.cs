// One-time editor construction for the current ArtSample, outside player compilation.
const string dir="Assets/Art/Contrast";
if(!UnityEditor.AssetDatabase.IsValidFolder(dir))UnityEditor.AssetDatabase.CreateFolder("Assets/Art","Contrast");
var remap=new System.Collections.Generic.Dictionary<UnityEngine.Material,UnityEngine.Material>();
System.Func<UnityEngine.Material,UnityEngine.Material> copy=m=>{if(m==null||!m.shader.name.StartsWith("Universal Render Pipeline/"))return m;if(remap.ContainsKey(m))return remap[m];string path=dir+"/"+m.name+".mat";var clone=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(path);if(clone==null){clone=new UnityEngine.Material(m);clone.name=m.name;UnityEditor.AssetDatabase.CreateAsset(clone,path);}remap[m]=clone;return clone;};
foreach(var r in UnityEngine.Object.FindObjectsByType<UnityEngine.Renderer>(UnityEngine.FindObjectsSortMode.None))r.sharedMaterials=r.sharedMaterials.Select(copy).ToArray();
foreach(var cat in new[]{"Energy","Bio","Tech"}){
 string path="Assets/Art/ReferenceSample/Prefabs/"+cat+"CargoSample.prefab";var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);try{
 root.transform.localScale=UnityEngine.Vector3.one*1.5f;
 foreach(var r in root.GetComponentsInChildren<UnityEngine.Renderer>())r.sharedMaterials=r.sharedMaterials.Select(copy).ToArray();
 var drag=new UnityEditor.SerializedObject(root.GetComponent<CargoDrag>());drag.FindProperty("dragHeight").floatValue=5.2f;drag.ApplyModifiedProperties();
 var drift=new UnityEditor.SerializedObject(root.GetComponent<CargoFloat>());drift.FindProperty("margin").floatValue=1.4f;drift.ApplyModifiedProperties();UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
 }finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
}
System.Func<string,UnityEngine.Material> mat=n=>UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(dir+"/"+n+".mat");
System.Action<string,UnityEngine.Color,float,float> paint=(n,c,metal,smooth)=>{var m=mat(n);if(m==null)return;m.SetColor("_BaseColor",c);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metal);if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);UnityEditor.EditorUtility.SetDirty(m);};
paint("Shell",new UnityEngine.Color(.055f,.085f,.13f),.45f,.32f);paint("Steel",new UnityEngine.Color(.40f,.51f,.63f),.42f,.4f);paint("Edge",new UnityEngine.Color(.64f,.75f,.82f),.4f,.35f);paint("Door",new UnityEngine.Color(.21f,.29f,.39f),.25f,.32f);
paint("Floor",new UnityEngine.Color(.055f,.078f,.115f),.16f,.24f);paint("FloorAlternate",new UnityEngine.Color(.05f,.07f,.105f),.16f,.24f);paint("FloorGap",new UnityEngine.Color(.015f,.025f,.045f),.2f,.2f);
paint("Energy",new UnityEngine.Color(.01f,.64f,.95f),.12f,.35f);paint("Bio",new UnityEngine.Color(.065f,.72f,.16f),.10f,.35f);paint("Tech",new UnityEngine.Color(.075f,.16f,.86f),.12f,.35f);paint("Inner",new UnityEngine.Color(.025f,.045f,.07f),.20f,.25f);
var back=mat("Back");back.shader=UnityEngine.Shader.Find("Universal Render Pipeline/Unlit");back.SetColor("_BaseColor",new UnityEngine.Color(.002f,.006f,.012f));UnityEditor.EditorUtility.SetDirty(back);
foreach(var n in new[]{"ColdLight","WarmAccent","SymbolInk"}){var m=mat(n);if(m==null)continue;m.SetColor("_EmissionColor",n=="ColdLight"?new UnityEngine.Color(2.0f,3.3f,4f):n=="WarmAccent"?new UnityEngine.Color(2.5f,.8f,.12f):new UnityEngine.Color(.5f,.6f,.7f));m.globalIlluminationFlags=UnityEngine.MaterialGlobalIlluminationFlags.BakedEmissive;m.EnableKeyword("_EMISSION");UnityEditor.EditorUtility.SetDirty(m);}
var bevel=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>("Assets/Art/Prototype/BeveledBlock.asset");
System.Func<UnityEngine.Transform,string,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Material,UnityEngine.Mesh,UnityEngine.Transform> part=(parent,n,p,s,m,mesh)=>{var g=new UnityEngine.GameObject(n);g.transform.SetParent(parent,false);g.transform.localPosition=p;g.transform.localScale=s;g.AddComponent<UnityEngine.MeshFilter>().sharedMesh=mesh;var r=g.AddComponent<UnityEngine.MeshRenderer>();r.sharedMaterial=m;r.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;return g.transform;};
var energy=UnityEngine.GameObject.Find("EnergyContainer");var template=energy.transform.Find("Visual");
foreach(var cat in new[]{"Energy","Bio","Tech"}){
 var root=UnityEngine.GameObject.Find(cat+"Container");var visual=root.transform.Find("Visual");if(cat!="Energy"){UnityEngine.Object.DestroyImmediate(visual.gameObject);visual=UnityEngine.Object.Instantiate(template,root.transform,false);visual.name="Visual";visual.localScale=new UnityEngine.Vector3(1/root.transform.localScale.x,1/root.transform.localScale.y,1/root.transform.localScale.z);visual.localPosition=new UnityEngine.Vector3(0,(1.9f-root.transform.position.y)/root.transform.localScale.y,0);}
 var h=visual.Find("Housing");h.localScale=new UnityEngine.Vector3(1.2f,1.5f,1.1f);
 foreach(var t in h.GetComponentsInChildren<UnityEngine.Transform>()){
 if(t.name.StartsWith("Pocket")||t.name.StartsWith("SideCategory")||t.name.StartsWith("Warning")){var p=t.localPosition;p.x=UnityEngine.Mathf.Sign(p.x)*2.43f;t.localPosition=p;var s=t.localScale;s.x=t.name.StartsWith("PocketPanel")?.56f:t.name=="PocketLeft"||t.name=="PocketRight"?.68f:t.name.StartsWith("SideCategory")?.45f:.30f;t.localScale=s;}
 }
 if(cat!="Energy"){
 foreach(var r in h.GetComponentsInChildren<UnityEngine.Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>m==mat("Energy")?mat(cat):m).ToArray();
 h.Find("CategoryLabel").GetComponent<TMPro.TMP_Text>().text=cat.ToUpper();h.Find("CategoryLabel").GetComponent<TMPro.TMP_Text>().color=cat=="Bio"?new UnityEngine.Color(.48f,1,.60f):new UnityEngine.Color(.48f,.65f,1);
 var symbol=h.Find("PrototypeArt/EnergySymbol");symbol.name="CategorySymbol";symbol.GetComponent<UnityEngine.MeshFilter>().sharedMesh=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>("Assets/Art/CargoBay/"+cat+"Symbol.asset");
 }
 var c=root.GetComponent<UnityEngine.BoxCollider>();c.size=new UnityEngine.Vector3(1.2f,1.2f,1.3f);
 var so=new UnityEditor.SerializedObject(root.GetComponent<ContainerFeedback>());so.FindProperty("leftDoor").objectReferenceValue=h.Find("LeftDoor");so.FindProperty("rightDoor").objectReferenceValue=h.Find("RightDoor");so.FindProperty("approachPoint").objectReferenceValue=h.Find("ApproachPoint");so.FindProperty("mouthPoint").objectReferenceValue=h.Find("MouthPoint");so.FindProperty("recessPoint").objectReferenceValue=h.Find("RecessPoint");so.FindProperty("intake").objectReferenceValue=c;so.ApplyModifiedProperties();
 var fill=h.Find("PrototypeArt/IntakeFill").GetComponent<UnityEngine.Light>();fill.transform.localPosition=new UnityEngine.Vector3(0,1.55f,-.9f);fill.intensity=7.5f;fill.range=5;fill.color=cat=="Bio"?new UnityEngine.Color(.55f,1,.72f):cat=="Tech"?new UnityEngine.Color(.55f,.65f,1):new UnityEngine.Color(.5f,.85f,1);fill.shadows=UnityEngine.LightShadows.None;
 part(h,"FixtureAboveAperture",new UnityEngine.Vector3(0,1.43f,-.48f),new UnityEngine.Vector3(1.10f,.10f,.10f),mat("ColdLight"),bevel);
}
// Replace the short test window by two long side openings, preserve the full BoxCollider.
var floor=UnityEngine.GameObject.Find("Floor").transform;var fa=floor.Find("FloorArt");
foreach(var t in fa.Cast<UnityEngine.Transform>().ToArray())if(t.name=="GlassSectionSample"||t.name=="WindowSurroundPanel"||t.name=="DeckPanel_0_3"||t.name=="DeckPanel_0_4"||t.name=="DeckPanel_3_3"||t.name=="DeckPanel_3_4")UnityEngine.Object.DestroyImmediate(t.gameObject);
var combines=new System.Collections.Generic.List<UnityEngine.CombineInstance>();var xs=new[]{-.5f,-.435f,-.315f,.315f,.435f,.5f};var zs=new[]{-.5f,-.125f,.125f,.5f};
for(int i=0;i<5;i++)for(int j=0;j<3;j++){if(j==1&&(i==1||i==3))continue;combines.Add(new UnityEngine.CombineInstance{mesh=bevel,transform=UnityEngine.Matrix4x4.TRS(new UnityEngine.Vector3((xs[i]+xs[i+1])/2,0,(zs[j]+zs[j+1])/2),UnityEngine.Quaternion.identity,new UnityEngine.Vector3(xs[i+1]-xs[i],1,zs[j+1]-zs[j]))});}
var mesh=new UnityEngine.Mesh();mesh.name="FloorTwoOpenings";mesh.CombineMeshes(combines.ToArray(),true,true);UnityEditor.AssetDatabase.CreateAsset(mesh,dir+"/FloorTwoOpenings.asset");floor.GetComponent<UnityEngine.MeshFilter>().sharedMesh=mesh;
var singlePlane=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>("Assets/Art/ReferenceSample/SingleGlassPlane.asset");
for(int side=-1;side<=1;side+=2){
 float cx=side*.375f;part(fa,"WindowSurround",new UnityEngine.Vector3(cx-.092f,.60f,0),new UnityEngine.Vector3(.0635f,.10f,.25f),mat("Floor"),bevel);part(fa,"WindowSurround",new UnityEngine.Vector3(cx+.092f,.60f,0),new UnityEngine.Vector3(.0635f,.10f,.25f),mat("Floor"),bevel);
 var w=new UnityEngine.GameObject(side<0?"GlassSectionLeft":"GlassSectionRight").transform;w.SetParent(fa,false);w.localPosition=new UnityEngine.Vector3(cx,.65f,0);w.localScale=new UnityEngine.Vector3(1f/24,5,1f/40);
 part(w,"SpaceThroughOpening",new UnityEngine.Vector3(0,-1.5f,0),new UnityEngine.Vector3(2.88f,.035f,10),mat("Space"),bevel);part(w,"TintedSinglePane",UnityEngine.Vector3.zero,new UnityEngine.Vector3(2.88f,1,10),mat("Glass"),singlePlane);
 var rand=new System.Random(side<0?41:82);for(int i=0;i<24;i++){float x=(float)(rand.NextDouble()-.5)*2.5f,z=(float)(rand.NextDouble()-.5)*9.5f;part(w,"StaticStar",new UnityEngine.Vector3(x,-1.42f,z),new UnityEngine.Vector3(i%7==0?.075f:.04f,.01f,i%7==0?.075f:.04f),mat("Stars"),bevel);}
 for(int s=-1;s<=1;s+=2){part(w,"ThickLongFrame",new UnityEngine.Vector3(s*1.48f,-.08f,0),new UnityEngine.Vector3(.27f,.65f,10.35f),mat("Steel"),bevel);part(w,"EndFrame",new UnityEngine.Vector3(0,-.08f,s*5.03f),new UnityEngine.Vector3(3.23f,.65f,.27f),mat("Steel"),bevel);part(w,"VisibleInteriorEdge",new UnityEngine.Vector3(s*1.31f,-.65f,0),new UnityEngine.Vector3(.12f,1.15f,10),mat("Inner"),bevel);part(w,"EdgeGlint",new UnityEngine.Vector3(s*1.33f,.015f,0),new UnityEngine.Vector3(.025f,.01f,9.8f),mat("Marking"),bevel);}
 for(int s=-1;s<=1;s+=2)part(w,"CrossMember",new UnityEngine.Vector3(0,-.025f,s*1.66f),new UnityEngine.Vector3(2.9f,.16f,.15f),mat("Steel"),bevel);
}
// Two actual unshadowed light pools, plus existing three intake fills. One shadow light total.
for(int side=-1;side<=1;side+=2){var wall=UnityEngine.GameObject.Find(side<0?"WallLeft":"WallRight").transform;var rig=new UnityEngine.GameObject("AccentFixture").transform;rig.SetParent(wall,false);rig.localPosition=new UnityEngine.Vector3(-side*.6f,.25f,-side*.15f);rig.localScale=new UnityEngine.Vector3(1/1.5f,1/3f,1/37f);
 part(rig,"LampHousing",UnityEngine.Vector3.zero,new UnityEngine.Vector3(.48f,1.50f,1.15f),mat("Shell"),bevel);part(rig,"VisibleEmitter",new UnityEngine.Vector3(-side*.25f,0,0),new UnityEngine.Vector3(.06f,1.15f,.68f),side<0?mat("ColdLight"):mat("WarmAccent"),bevel);
 var g=new UnityEngine.GameObject("SurfacePoolLight");g.transform.SetParent(rig,false);g.transform.localPosition=new UnityEngine.Vector3(-side*1.2f,-.25f,0);var light=g.AddComponent<UnityEngine.Light>();light.type=UnityEngine.LightType.Point;light.color=side<0?new UnityEngine.Color(.5f,.78f,1):new UnityEngine.Color(1,.65f,.30f);light.intensity=side<0?25:18;light.range=6;light.shadows=UnityEngine.LightShadows.None;
}
var sun=UnityEngine.GameObject.Find("Directional Light").GetComponent<UnityEngine.Light>();sun.intensity=2.15f;sun.shadowStrength=.58f;UnityEngine.RenderSettings.ambientSkyColor=new UnityEngine.Color(.26f,.32f,.42f);UnityEngine.RenderSettings.ambientEquatorColor=new UnityEngine.Color(.13f,.17f,.24f);
var sp=new UnityEditor.SerializedObject(UnityEngine.Object.FindFirstObjectByType<CargoSpawner>());sp.FindProperty("cargoY").floatValue=4.9f;sp.FindProperty("edgeMargin").floatValue=1.4f;sp.FindProperty("minDistance").floatValue=3.1f;sp.ApplyModifiedProperties();
UnityEditor.AssetDatabase.SaveAssets();UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());return "ArtSample: contrast, six lights/one shadow, two glass openings, cargo1.5, aperture3.84x3.0; camera/layout unchanged.";

