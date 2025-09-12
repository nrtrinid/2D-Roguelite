using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize = 32;
    private readonly Queue<GameObject> pool = new();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }
    public GameObject Get()
    {
        if (pool.Count > 0) { var go = pool.Dequeue(); go.SetActive(true); return go; }
        return Instantiate(prefab, transform);
    }
    public void ReturnToPool(GameObject go)
    {
        go.SetActive(false);
        pool.Enqueue(go);
    }
}
