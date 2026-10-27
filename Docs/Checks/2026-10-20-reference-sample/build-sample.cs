// Editor-only construction recipe, outside Assets and player compilation.
const string dir="Assets/Art/ReferenceSample";
if(!UnityEditor.AssetDatabase.IsValidFolder(dir))UnityEditor.AssetDatabase.CreateFolder("Assets/Art","ReferenceSample");
if(!UnityEditor.AssetDatabase.IsValidFolder(dir+"/Prefabs"))UnityEditor.AssetDatabase.CreateFolder(dir,"Prefabs");
if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name!="ArtSample")UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),"Assets/Scenes/ArtSample.unity");
System.Func<string,UnityEngine.Material> load=p=>UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(p);
var shell=load("Assets/Art/Prototype/Shell.mat");var steel=load("Assets/Art/Prototype/Steel.mat");var edge=load("Assets/Art/Prototype/Edge.mat");var cold=load("Assets/Art/Prototype/ColdLight.mat");var warm=load("Assets/Art/Prototype/WarmAccent.mat");var ink=load("Assets/Art/CargoBay/SymbolInk.mat");var floorMat=load("Assets/Art/CargoBay/Floor.mat");
System.Func<string,UnityEngine.Color,float,UnityEngine.Material> material=(n,c,metal)=>{var m=load(dir+"/"+n+".mat");if(m==null){m=new UnityEngine.Material(UnityEngine.Shader.Find("Universal Render Pipeline/Lit"));UnityEditor.AssetDatabase.CreateAsset(m,dir+"/"+n+".mat");}m.SetColor("_BaseColor",c);m.SetFloat("_Metallic",metal);m.SetFloat("_Smoothness",.48f);m.SetFloat("_Cull",0);m.enableInstancing=true;UnityEditor.EditorUtility.SetDirty(m);return m;};
var glass=material("Glass",new UnityEngine.Color(.20f,.55f,.72f,.17f),.15f);glass.SetFloat("_Surface",1);glass.SetFloat("_Blend",0);glass.SetFloat("_SrcBlend",5);glass.SetFloat("_DstBlend",10);glass.SetFloat("_ZWrite",0);glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");glass.renderQueue=3000;glass.SetOverrideTag("RenderType","Transparent");
var space=material("Space",new UnityEngine.Color(.003f,.007f,.018f),0);var star=material("Stars",new UnityEngine.Color(.26f,.39f,.52f),0);star.SetColor("_EmissionColor",new UnityEngine.Color(.20f,.29f,.4f));star.EnableKeyword("_EMISSION");star.globalIlluminationFlags=UnityEngine.MaterialGlobalIlluminationFlags.BakedEmissive;
var pale=material("PaintPale",new UnityEngine.Color(.58f,.68f,.72f),.25f);var green=load("Assets/Art/CargoBay/Bio.mat");var cyan=load("Assets/Art/Prototype/Energy.mat");var blue=load("Assets/Art/CargoBay/Tech.mat");
var bevel=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>("Assets/Art/Prototype/BeveledBlock.asset");
System.Func<string,UnityEngine.Mesh,UnityEngine.Mesh> saveMesh=(n,m)=>{var old=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(dir+"/"+n+".asset");if(old==null){UnityEditor.AssetDatabase.CreateAsset(m,dir+"/"+n+".asset");return m;}UnityEditor.EditorUtility.CopySerialized(m,old);UnityEngine.Object.DestroyImmediate(m);return old;};
System.Func<string,float[],float[],UnityEngine.Mesh> lathe=(name,ys,rs)=>{var vs=new System.Collections.Generic.List<UnityEngine.Vector3>();var ts=new System.Collections.Generic.List<int>();int segments=20;for(int j=0;j<ys.Length;j++)for(int i=0;i<=segments;i++){float a=i*UnityEngine.Mathf.PI*2/segments;vs.Add(new UnityEngine.Vector3(UnityEngine.Mathf.Sin(a)*rs[j],ys[j],UnityEngine.Mathf.Cos(a)*rs[j]));}for(int j=0;j<ys.Length-1;j++)for(int i=0;i<segments;i++){int a=j*(segments+1)+i,b=a+segments+1;ts.AddRange(new[]{a,a+1,b,a+1,b+1,b});}var m=new UnityEngine.Mesh();m.name=name;m.SetVertices(vs);m.SetTriangles(ts,0);m.RecalculateNormals();m.RecalculateBounds();return saveMesh(name,m);};
var cylinder=lathe("EnergyBody",new[]{-.5f,-.48f,-.41f,.41f,.48f,.5f},new[]{0f,.36f,.43f,.43f,.36f,0f});
var ring=lathe("Collar",new[]{-.055f,-.04f,.04f,.055f},new[]{.39f,.48f,.48f,.39f});
var bioBottom=lathe("BioLower",new[]{-.5f,-.47f,-.40f,-.28f,-.15f},new[]{0f,.24f,.37f,.44f,.45f});
var bioTop=lathe("BioUpper",new[]{.15f,.28f,.40f,.47f,.5f},new[]{.45f,.44f,.37f,.24f,0f});
var bioWindow=lathe("BioWindow",new[]{-.15f,.15f},new[]{.45f,.45f});
System.Func<UnityEngine.Transform,string,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Material,UnityEngine.Mesh,UnityEngine.Transform> part=(parent,n,p,s,mat,mesh)=>{var go=new UnityEngine.GameObject(n);go.transform.SetParent(parent,false);go.transform.localPosition=p;go.transform.localScale=s;go.AddComponent<UnityEngine.MeshFilter>().sharedMesh=mesh;var r=go.AddComponent<UnityEngine.MeshRenderer>();r.sharedMaterial=mat;r.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.On;return go.transform;};
// Variant prefabs: gameplay components/tags kept, only local Visual and fitting collider differ.
foreach(var category in new[]{"Energy","Bio","Tech"}){
 string path=dir+"/Prefabs/"+category+"CargoSample.prefab";if(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path)==null)UnityEditor.AssetDatabase.CopyAsset("Assets/Prefabs/"+category+"Cargo.prefab",path);
 var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);try{
 var visual=root.transform.Find("Visual");for(int i=visual.childCount-1;i>=0;i--)UnityEngine.Object.DestroyImmediate(visual.GetChild(i).gameObject);var renderer=visual.GetComponent<UnityEngine.Renderer>();renderer.enabled=false;
 root.transform.localScale=UnityEngine.Vector3.one*1.2f;visual.localScale=UnityEngine.Vector3.one;
 var box=root.GetComponent<UnityEngine.BoxCollider>();if(box!=null)UnityEngine.Object.DestroyImmediate(box);
 var cap=root.GetComponent<UnityEngine.CapsuleCollider>();if(cap!=null)UnityEngine.Object.DestroyImmediate(cap);
 if(category=="Energy"){
 var c=root.AddComponent<UnityEngine.CapsuleCollider>();c.direction=1;c.radius=.45f;c.height=1.06f;
 part(visual,"CyanPowerCore",UnityEngine.Vector3.zero,UnityEngine.Vector3.one,cyan,cylinder);
 for(int side=-1;side<=1;side+=2){part(visual,"EndCollar",new UnityEngine.Vector3(0,side*.38f,0),UnityEngine.Vector3.one,pale,ring);part(visual,"Terminal",new UnityEngine.Vector3(0,side*.49f,0),new UnityEngine.Vector3(.50f,.07f,.5f),shell,bevel);}
 for(int i=0;i<4;i++){float a=i*90*UnityEngine.Mathf.Deg2Rad;var rib=part(visual,"LongitudinalGuard",new UnityEngine.Vector3(UnityEngine.Mathf.Sin(a)*.42f,0,UnityEngine.Mathf.Cos(a)*.42f),new UnityEngine.Vector3(.08f,.59f,.07f),steel,bevel);rib.localRotation=UnityEngine.Quaternion.Euler(0,i*90,0);}
 }else if(category=="Bio"){
 var c=root.AddComponent<UnityEngine.CapsuleCollider>();c.direction=1;c.radius=.45f;c.height=1f;
 part(visual,"RoundedLowerShell",UnityEngine.Vector3.zero,UnityEngine.Vector3.one,green,bioBottom);part(visual,"RoundedUpperShell",UnityEngine.Vector3.zero,UnityEngine.Vector3.one,green,bioTop);
 var window=part(visual,"GlassObservationBand",UnityEngine.Vector3.zero,UnityEngine.Vector3.one,glass,bioWindow);window.GetComponent<UnityEngine.Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
 for(int side=-1;side<=1;side+=2)part(visual,"WindowCollar",new UnityEngine.Vector3(0,side*.17f,0),new UnityEngine.Vector3(.97f,.65f,.97f),pale,ring);
 part(visual,"SpecimenStem",UnityEngine.Vector3.zero,new UnityEngine.Vector3(.05f,.35f,.05f),green,bevel);
 var leaf=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>("Assets/Art/CargoBay/BioSymbol.asset");var plant=part(visual,"SpecimenLeaf",new UnityEngine.Vector3(.06f,0,-.04f),new UnityEngine.Vector3(.3f,.30f,.3f),green,leaf);plant.localRotation=UnityEngine.Quaternion.Euler(0,20,-30);
 }else{
 var c=root.AddComponent<UnityEngine.BoxCollider>();c.size=UnityEngine.Vector3.one;
 part(visual,"AngularCase",UnityEngine.Vector3.zero,UnityEngine.Vector3.one,blue,bevel);
 for(int x=-1;x<=1;x+=2)for(int y=-1;y<=1;y+=2)for(int z=-1;z<=1;z+=2)part(visual,"CornerArmor",new UnityEngine.Vector3(x*.39f,y*.39f,z*.39f),UnityEngine.Vector3.one*.25f,steel,bevel);
 part(visual,"TopInset",new UnityEngine.Vector3(0,.506f,0),new UnityEngine.Vector3(.64f,.035f,.64f),shell,bevel);
 }
 // Matching glyphs on opposite broad sides, always part of the deforming Visual.
 var glyph=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(category=="Energy"?"Assets/Art/Prototype/EnergySymbol.asset":"Assets/Art/CargoBay/"+category+"Symbol.asset");
 for(int side=-1;side<=1;side+=2){float yy=category=="Bio"?.32f:0;var badge=part(visual,"CategoryBadge",new UnityEngine.Vector3(0,yy,side*(category=="Tech"?.512f:.435f)),new UnityEngine.Vector3(.48f,.32f,.025f),category=="Tech"?blue:shell,bevel);var g=part(visual,"CategorySymbol",new UnityEngine.Vector3(0,yy,side*(category=="Tech"?.532f:.456f)),UnityEngine.Vector3.one*.29f,ink,glyph);if(side==1)g.localRotation=UnityEngine.Quaternion.Euler(0,180,0);}
 UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
 }finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
}
var spawner=UnityEngine.Object.FindFirstObjectByType<CargoSpawner>();var so=new UnityEditor.SerializedObject(spawner);foreach(var cat in new[]{"Energy","Bio","Tech"})so.FindProperty(cat.ToLower()+"Prefab").objectReferenceValue=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(dir+"/Prefabs/"+cat+"CargoSample.prefab");so.ApplyModifiedProperties();
// Energy hatch sample only: stronger outer shell, deeper lining, visible corner armor.
var housing=UnityEngine.GameObject.Find("EnergyContainer").transform.Find("Visual/Housing");housing.localRotation=UnityEngine.Quaternion.Euler(48,0,0);
var prior=housing.Find("ReferenceArt");if(prior!=null)UnityEngine.Object.DestroyImmediate(prior.gameObject);var art=new UnityEngine.GameObject("ReferenceArt").transform;art.SetParent(housing,false);
for(int side=-1;side<=1;side+=2){part(art,"HeavyJamb",new UnityEngine.Vector3(side*1.96f,0,-.05f),new UnityEngine.Vector3(.44f,2.95f,.8f),steel,bevel);part(art,"InnerJambEdge",new UnityEngine.Vector3(side*1.65f,0,.50f),new UnityEngine.Vector3(.10f,1.92f,.9f),edge,bevel);for(int y=-1;y<=1;y+=2){var k=part(art,"ChamferedCorner",new UnityEngine.Vector3(side*1.72f,y*1.20f,-.45f),new UnityEngine.Vector3(.65f,.27f,.20f),edge,bevel);k.localRotation=UnityEngine.Quaternion.Euler(0,0,-side*y*35);}}
part(art,"ProjectingLintel",new UnityEngine.Vector3(0,1.40f,-.03f),new UnityEngine.Vector3(3.5f,.40f,.82f),steel,bevel);
part(art,"DeepSill",new UnityEngine.Vector3(0,-1.22f,.55f),new UnityEngine.Vector3(3.16f,.15f,1.45f),steel,bevel);
part(art,"BackWarning",new UnityEngine.Vector3(0,-.40f,1.91f),new UnityEngine.Vector3(1.8f,.09f,.05f),warm,bevel);
housing.Find("InnerWalls").localScale=new UnityEngine.Vector3(1,1,1.25f);housing.Find("BackSurface").localPosition=new UnityEngine.Vector3(0,0,2.06f);housing.Find("RecessPoint").localPosition=new UnityEngine.Vector3(0,0,1.9f);
// Real visual floor hole, full existing BoxCollider remains continuous.
var floor=UnityEngine.GameObject.Find("Floor").transform;var fa=floor.Find("FloorArt");var tile=fa.Find("DeckPanel_0_3");if(tile!=null)UnityEngine.Object.DestroyImmediate(tile.gameObject);
float x0=-.435f,x1=-.315f,z0=-.1075f,z1=-.0175f;
var combines=new System.Collections.Generic.List<UnityEngine.CombineInstance>();
System.Action<float,float,float,float> slab=(ax,bx,az,bz)=>{combines.Add(new UnityEngine.CombineInstance{mesh=bevel,transform=UnityEngine.Matrix4x4.TRS(new UnityEngine.Vector3((ax+bx)/2,0,(az+bz)/2),UnityEngine.Quaternion.identity,new UnityEngine.Vector3(bx-ax,1,bz-az))});};
slab(-.5f,x0,-.5f,.5f);slab(x1,.5f,-.5f,.5f);slab(x0,x1,-.5f,z0);slab(x0,x1,z1,.5f);var cut=new UnityEngine.Mesh();cut.CombineMeshes(combines.ToArray(),true,true);floor.GetComponent<UnityEngine.MeshFilter>().sharedMesh=saveMesh("FloorWithActualOpening",cut);
// Replacement tile strips meet the opening; no hidden full tile underneath.
System.Action<float,float,float,float> patch=(ax,bx,az,bz)=>part(fa,"WindowSurroundPanel",new UnityEngine.Vector3((ax+bx)/2,.60f,(az+bz)/2),new UnityEngine.Vector3(bx-ax,.1f,bz-az),floorMat,bevel);
patch(-.49875f,x0,-.12425f,-.00075f);patch(x1,-.25125f,-.12425f,-.00075f);patch(x0,x1,-.12425f,z0);patch(x0,x1,z1,-.00075f);
var oldWindow=fa.Find("GlassSectionSample");if(oldWindow!=null)UnityEngine.Object.DestroyImmediate(oldWindow.gameObject);var w=new UnityEngine.GameObject("GlassSectionSample").transform;w.SetParent(fa,false);w.localPosition=new UnityEngine.Vector3(-.375f,.65f,-.0625f);w.localScale=new UnityEngine.Vector3(1f/24,5,1f/40);
part(w,"SpaceVisibleThroughOpening",new UnityEngine.Vector3(0,-1.5f,0),new UnityEngine.Vector3(2.88f,.035f,3.6f),space,bevel);
var rand=new System.Random(41);for(int i=0;i<11;i++){float x=(float)(rand.NextDouble()-.5)*2.5f,z=(float)(rand.NextDouble()-.5)*3.25f;var st=part(w,"StaticStar",new UnityEngine.Vector3(x,-1.42f,z),new UnityEngine.Vector3(i%4==0?.045f:.025f,.012f,i%4==0?.045f:.025f),star,bevel);st.GetComponent<UnityEngine.Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;}
var pane=part(w,"BlueGlass",UnityEngine.Vector3.zero,new UnityEngine.Vector3(2.88f,.018f,3.6f),glass,bevel);pane.GetComponent<UnityEngine.Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
for(int side=-1;side<=1;side+=2){part(w,"ThickFrameLong",new UnityEngine.Vector3(side*1.47f,-.10f,0),new UnityEngine.Vector3(.23f,.5f,3.95f),steel,bevel);part(w,"ThickFrameEnd",new UnityEngine.Vector3(0,-.1f,side*1.82f),new UnityEngine.Vector3(3.16f,.5f,.23f),steel,bevel);part(w,"InteriorEdge",new UnityEngine.Vector3(side*1.34f,-.65f,0),new UnityEngine.Vector3(.09f,1.15f,3.6f),shell,bevel);}
part(w,"CrossBrace",new UnityEngine.Vector3(0,-.03f,0),new UnityEngine.Vector3(2.9f,.16f,.14f),steel,bevel);part(w,"GlassSoftGlint",new UnityEngine.Vector3(.98f,.025f,.2f),new UnityEngine.Vector3(.018f,.012f,2.8f),load("Assets/Art/CargoBay/Marking.mat"),bevel);
// One perimeter sample: layered buttresses and a diagonally chamfered top corner.
var wall=UnityEngine.GameObject.Find("WallLeft").transform;var old=wall.Find("ReferenceSection");if(old!=null)UnityEngine.Object.DestroyImmediate(old.gameObject);var ws=new UnityEngine.GameObject("ReferenceSection").transform;ws.SetParent(wall,false);ws.localPosition=new UnityEngine.Vector3(.28f,0,.30f);ws.localScale=new UnityEngine.Vector3(1/1.5f,1/3f,1/37f);
part(ws,"LayeredWallBase",new UnityEngine.Vector3(0,0,0),new UnityEngine.Vector3(1.0f,2.5f,8.5f),shell,bevel);
for(int i=-1;i<=1;i++){part(ws,"ArmoredButtress",new UnityEngine.Vector3(.4f,.1f,i*3.3f),new UnityEngine.Vector3(.8f,3.25f,.75f),steel,bevel);part(ws,"ShoulderCap",new UnityEngine.Vector3(.4f,1.8f,i*3.3f),new UnityEngine.Vector3(1.25f,.45f,1.3f),edge,bevel);part(ws,"RecessedLamp",new UnityEngine.Vector3(.84f,.45f,i*3.3f),new UnityEngine.Vector3(.06f,1.25f,.24f),i==0?cold:warm,bevel);}
var corner=part(ws,"SlopedCorner",new UnityEngine.Vector3(.4f,.2f,6.65f),new UnityEngine.Vector3(1.8f,3.3f,3.2f),steel,bevel);corner.localRotation=UnityEngine.Quaternion.Euler(0,35,0);
// Lighting trial in this scene only; no extra dynamic light.
var sun=UnityEngine.GameObject.Find("Directional Light").GetComponent<UnityEngine.Light>();sun.intensity=1.7f;sun.shadowStrength=.50f;sun.transform.rotation=UnityEngine.Quaternion.Euler(48,325,0);UnityEngine.RenderSettings.ambientSkyColor=new UnityEngine.Color(.30f,.37f,.47f);UnityEngine.RenderSettings.ambientEquatorColor=new UnityEngine.Color(.17f,.22f,.30f);
UnityEditor.AssetDatabase.SaveAssets();UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
return "ArtSample saved; one Energy hatch/perimeter/glass section and three variant cargo prefabs. Main remains unchanged.";

