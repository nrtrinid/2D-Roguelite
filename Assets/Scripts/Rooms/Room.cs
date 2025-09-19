using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [HideInInspector] public RoomSequencer sequencer;

    public Transform playerEntrance;
    public Transform playerEntranceBack;
    public Door exitDoor;
    public Door backDoor;
    public EnemyManager enemyManager;
    public PolygonCollider2D confinerBounds;

    [Header("Chests controlled by this Room")]
    public bool lockChestsUntilClear = true;
    public List<Chest> chests = new();   // assign in prefab OR auto-find

    void Awake()
    {
        if (!enemyManager) enemyManager = GetComponentInChildren<EnemyManager>(true);
        if (enemyManager) enemyManager.onAllDead += OnAllDead;

        if (exitDoor) exitDoor.Lock(true);
        if (backDoor) backDoor.Lock(false);

        // Auto-find chests if list is empty (optional)
        if (chests == null || chests.Count == 0)
        {
            chests = new List<Chest>(GetComponentsInChildren<Chest>(true));
        }

        // Ensure each chest knows its room & initial lock state
        foreach (var c in chests)
        {
            if (!c) continue;
            c.room = this;
            c.Lock(lockChestsUntilClear);   // locked at start if combat room, else unlocked (hub/puzzle control)
        }
    }

    public void BeginRoom() { enemyManager?.Activate(); }

    void OnAllDead()
    {
        if (exitDoor) exitDoor.Lock(false);
        if (lockChestsUntilClear)
        {
            foreach (var c in chests) if (c) c.Lock(false);  // unlock on clear
        }
    }

    public void UseExit() { sequencer?.NextRoom(); }
    public void UseBack() { sequencer?.PrevRoom(); }
}