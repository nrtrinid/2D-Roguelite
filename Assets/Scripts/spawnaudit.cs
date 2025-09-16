using UnityEngine;
public class SpawnAudit : MonoBehaviour
{
    static int counter; public int id;
    void Awake(){ id = ++counter; Debug.Log($"[SpawnAudit] AWAKE {name} id={id} inst={GetInstanceID()}"); }
    void OnEnable(){ Debug.Log($"[SpawnAudit] ENABLE {name} id={id}"); }
}
