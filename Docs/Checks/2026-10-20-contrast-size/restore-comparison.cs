UnityEngine.Application.runInBackground=true;
foreach(var c in UnityEngine.Object.FindObjectsByType<CargoCollector>(UnityEngine.FindObjectsSortMode.None))UnityEngine.Object.DestroyImmediate(c.gameObject);
var entries=Newtonsoft.Json.Linq.JArray.Parse(System.IO.File.ReadAllText("Docs/Checks/2026-10-20-contrast-size/poses.json"));
var list=new System.Collections.Generic.List<CargoCollector>();
foreach(var e in entries){string category=(string)e["category"];var prefab=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>("Assets/Art/ReferenceSample/Prefabs/"+category+"CargoSample.prefab");var g=UnityEngine.Object.Instantiate(prefab,new UnityEngine.Vector3((float)e["x"],(float)e["y"],(float)e["z"]),UnityEngine.Quaternion.Euler((float)e["rx"],(float)e["ry"],(float)e["rz"]));g.GetComponent<CargoFloat>().enabled=false;g.GetComponent<CargoRepulsion>().enabled=false;g.GetComponent<UnityEngine.Rigidbody>().isKinematic=true;list.Add(g.GetComponent<CargoCollector>());}
var level=LevelManager.Instance;var flags=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;
((System.Collections.Generic.HashSet<CargoCollector>)typeof(LevelManager).GetField("remaining",flags).GetValue(level)).Clear();
typeof(LevelManager).GetField("<IsPrepared>k__BackingField",flags).SetValue(level,false);level.PrepareLevel(list);
var cam=UnityEngine.Camera.main;string json=Newtonsoft.Json.JsonConvert.SerializeObject(new{position=cam.transform.position.ToString("F6"),rotation=cam.transform.eulerAngles.ToString("F6"),cam.orthographicSize,cam.aspect,width=UnityEngine.Screen.width,height=UnityEngine.Screen.height});System.IO.File.WriteAllText("Docs/Checks/2026-10-20-contrast-size/camera-after.json",json);
return new{cameraExactlyEqual=json==System.IO.File.ReadAllText("Docs/Checks/2026-10-20-contrast-size/camera-before.json"),stagedItems=list.Count};
