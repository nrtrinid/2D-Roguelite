using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize = 32;
    readonly Queue<GameObject> pool = new();
    Vector3 prefabScale;

    void Awake()
    {
        prefabScale = prefab.transform.localScale;
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(prefab, transform);
            go.transform.localScale = prefabScale;
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    public GameObject Get()
    {
        var go = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, transform);
        go.transform.SetParent(null, true);          // optional: detach to world
        go.transform.localScale = prefabScale;       // <<< ensure correct size
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);
        return go;
    }

    public void ReturnToPool(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(transform, false);
        go.transform.localScale = prefabScale;       // <<< restore, not Vector3.one
        go.transform.rotation = Quaternion.identity;
        pool.Enqueue(go);
    }
}
