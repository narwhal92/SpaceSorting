foreach(var cat in new[]{"Energy","Bio","Tech"}){
 string path="Assets/Art/ReferenceSample/Prefabs/"+cat+"CargoSample.prefab";var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);
 try{root.transform.localScale=UnityEngine.Vector3.one*1.25f;
 var visual=root.transform.Find("Visual");var ink=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>("Assets/Art/CargoBay/SymbolInk.mat");
 var paint=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(cat=="Energy"?"Assets/Art/Prototype/Energy.mat":"Assets/Art/CargoBay/"+cat+".mat");
 foreach(var t in visual.GetComponentsInChildren<UnityEngine.Transform>()){
 if(t.name=="EndCollar")t.GetComponent<UnityEngine.Renderer>().sharedMaterial=paint;
 if(t.name=="Terminal"){t.GetComponent<UnityEngine.Renderer>().sharedMaterial=paint;t.localScale=new UnityEngine.Vector3(.60f,.07f,.60f);}
 if(t.name=="LongitudinalGuard")t.localScale=new UnityEngine.Vector3(.045f,.52f,.045f);
 if(cat=="Bio"&&t.name=="CategoryBadge")t.GetComponent<UnityEngine.Renderer>().sharedMaterial=paint;
 if(cat=="Tech"&&t.name=="CornerArmor")t.localScale=UnityEngine.Vector3.one*.20f;
 }
 var glyph=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(cat=="Energy"?"Assets/Art/Prototype/EnergySymbol.asset":"Assets/Art/CargoBay/"+cat+"Symbol.asset");
 for(int side=-1;side<=1;side+=2){var g=new UnityEngine.GameObject("EndCategorySymbol");g.transform.SetParent(visual,false);g.transform.localPosition=new UnityEngine.Vector3(0,side*.54f,0);g.transform.localRotation=UnityEngine.Quaternion.Euler(side*90,0,0);g.transform.localScale=UnityEngine.Vector3.one*.32f;g.AddComponent<UnityEngine.MeshFilter>().sharedMesh=glyph;g.AddComponent<UnityEngine.MeshRenderer>().sharedMaterial=ink;}
 var s=new UnityEditor.SerializedObject(root.GetComponent<CargoFloat>());s.FindProperty("margin").floatValue=cat=="Tech"?1.1f:.95f;s.ApplyModifiedProperties();
 UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
 }finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
}
UnityEditor.AssetDatabase.SaveAssets();return "Sample cargo +25%, stronger colored caps, top/bottom glyphs, fitting drift margins.";
