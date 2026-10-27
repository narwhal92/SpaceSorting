var flags=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;
var grab=typeof(CargoDrag).GetMethod("OnMouseDown",flags);
var release=typeof(CargoDrag).GetMethod("OnMouseUp",flags);
var items=UnityEngine.Object.FindObjectsByType<CargoCollector>(UnityEngine.FindObjectsSortMode.None);
foreach(var c in items){c.GetComponent<CargoFloat>().enabled=false;c.GetComponent<CargoRepulsion>().enabled=false;c.GetComponent<UnityEngine.Rigidbody>().isKinematic=true;typeof(CargoDrag).GetField("mainCamera",flags).SetValue(c.GetComponent<CargoDrag>(),null);}
var log=new System.Collections.Generic.List<string>();
var level=LevelManager.Instance;
log.Add("initial="+level.DeliveredCargo+"/"+level.TotalCargo+"; actual="+items.Length);
var first=items[0];var wrong=UnityEngine.Object.FindObjectsByType<ContainerFeedback>(UnityEngine.FindObjectsSortMode.None).First(h=>!h.Matches(first));
grab.Invoke(first.GetComponent<CargoDrag>(),null);first.transform.position=wrong.transform.position+new UnityEngine.Vector3(0,4.3f,0);release.Invoke(first.GetComponent<CargoDrag>(),null);
log.Add("wrongSurvives="+(!first.IsCollecting&&level.DeliveredCargo==0));
log.Add("freeReleaseRejected="+(!first.TryRelease()));
int index=0;float next=UnityEngine.Time.time+.5f;bool waiting=false;bool duplicates=true;bool countDeferred=true;bool finishDeferred=true;
UnityEditor.EditorApplication.CallbackFunction tick=null;
tick=()=>{
 if(!UnityEditor.EditorApplication.isPlaying){UnityEditor.EditorApplication.update-=tick;return;}
 if(UnityEngine.Time.time<next)return;
 if(waiting){if(items[index]!=null)return;index++;waiting=false;}
 if(index>=items.Length){log.Add("duplicateRejected="+duplicates);log.Add("countDeferred="+countDeferred);log.Add("lastCompletionDeferred="+finishDeferred);log.Add("final="+level.DeliveredCargo+"/"+level.TotalCargo+" complete="+level.IsComplete);log.Add("remaining="+UnityEngine.Object.FindObjectsByType<CargoCollector>(UnityEngine.FindObjectsSortMode.None).Length);log.Add("postCompleteRejected="+(!level.TryDeliver(null)));System.IO.File.WriteAllLines("Docs/Checks/2026-10-20-reference-sample/loop-validation.txt",log);UnityEditor.EditorApplication.update-=tick;return;}
 var c=items[index];var d=c.GetComponent<CargoDrag>();var h=UnityEngine.Object.FindObjectsByType<ContainerFeedback>(UnityEngine.FindObjectsSortMode.None).First(x=>x.Matches(c));
 int before=level.DeliveredCargo;grab.Invoke(d,null);c.transform.position=h.transform.position+new UnityEngine.Vector3(0,4.3f,-.2f);release.Invoke(d,null);duplicates&=!c.TryRelease();countDeferred&=level.DeliveredCargo==before; if(index==items.Length-1)finishDeferred&=!level.IsComplete;
 log.Add("started="+c.tag+" collecting="+c.IsCollecting+" colliderOff="+c.GetComponentsInChildren<UnityEngine.Collider>().All(x=>!x.enabled));waiting=true;next=UnityEngine.Time.time+.7f;
};
UnityEditor.EditorApplication.update+=tick;return "Started scripted full-loop regression; not actual mouse input.";

