using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Chest : MonoBehaviour
{
    public Room room;
    public GameObject closedVisual;
    public GameObject openVisual;

    [Header("Loot Settings")]
    [Tooltip("Each entry in the list will spawn once. Duplicate entries = more drops.")]
    public List<GameObject> lootPrefabs = new();
    public float scatterRadius = 1.2f; // how far from chest center

    public bool locked = true;
    bool opened = false;

    void Reset() { GetComponent<Collider2D>().isTrigger = true; }
    void Start() { RefreshVisuals(); }

    public void Lock(bool state) => locked = state;

    void RefreshVisuals()
    {
        if (closedVisual) closedVisual.SetActive(!opened);
        if (openVisual)   openVisual.SetActive(opened);
    }

    void Open()
    {
        opened = true;
        RefreshVisuals();
        DropLoot();
    }

    void DropLoot()
    {
        if (lootPrefabs == null || lootPrefabs.Count == 0) return;

        foreach (var prefab in lootPrefabs)
        {
            if (!prefab) continue;

            // random scatter point in circle
            Vector2 offset = Random.insideUnitCircle * scatterRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);

            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (locked || opened) return;
        Open();
    }
}
