// Bake static decorations by shared material. No runtime component or dependency.
foreach(var cat in new[]{"Energy","Bio","Tech"}){
 var path="Assets/Art/ReferenceSample/Prefabs/"+cat+"CargoSample.prefab";var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);
 try{var visual=root.transform.Find("Visual");var renderers=visual.GetComponentsInChildren<UnityEngine.MeshRenderer>().Where(r=>r.enabled).ToArray();var groups=renderers.GroupBy(r=>r.sharedMaterial).ToArray();int index=0;
 foreach(var group in groups){var instances=group.Select(r=>new UnityEngine.CombineInstance{mesh=r.GetComponent<UnityEngine.MeshFilter>().sharedMesh,transform=visual.worldToLocalMatrix*r.transform.localToWorldMatrix}).ToArray();var mesh=new UnityEngine.Mesh();mesh.name=cat+"Combined"+index;mesh.CombineMeshes(instances,true,true);string mp="Assets/Art/ReferenceSample/"+mesh.name+".asset";var old=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(mp);if(old==null)UnityEditor.AssetDatabase.CreateAsset(mesh,mp);else{UnityEditor.EditorUtility.CopySerialized(mesh,old);UnityEngine.Object.DestroyImmediate(mesh);mesh=old;}
 var g=new UnityEngine.GameObject("Combined_"+group.Key.name);g.transform.SetParent(visual,false);g.AddComponent<UnityEngine.MeshFilter>().sharedMesh=mesh;var r=g.AddComponent<UnityEngine.MeshRenderer>();r.sharedMaterial=group.Key;if(group.Key.name=="Glass")r.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;index++;}
 foreach(var r in renderers)UnityEngine.Object.DestroyImmediate(r.gameObject);
 var oldRenderer=visual.GetComponent<UnityEngine.MeshRenderer>();if(oldRenderer!=null)UnityEngine.Object.DestroyImmediate(oldRenderer);var oldFilter=visual.GetComponent<UnityEngine.MeshFilter>();if(oldFilter!=null)UnityEngine.Object.DestroyImmediate(oldFilter);
 UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
 }finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
}
UnityEditor.AssetDatabase.SaveAssets();return "Static cargo geometry combined by material, four renderers per variant; colliders and Visual transforms preserved.";
