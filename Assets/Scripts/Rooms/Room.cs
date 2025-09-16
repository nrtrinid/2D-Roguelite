// Room.cs
using UnityEngine;
using Unity.Cinemachine;

public class Room : MonoBehaviour
{
    [HideInInspector] public RoomSequencer sequencer;

    public Transform playerEntrance;      // coming from previous (forward)
    public Transform playerEntranceBack;  // coming from next (backtracking)
    public Door exitDoor;                 // forward door
    public Door backDoor;                 // optional: backward door (can be null)
    public EnemyManager enemyManager;
    public PolygonCollider2D confinerBounds;

    void Awake()
    {
        if (!enemyManager) enemyManager = GetComponentInChildren<EnemyManager>(true);
        if (enemyManager) enemyManager.onAllDead += OnAllDead;

        if (exitDoor) exitDoor.Lock(true);
        if (backDoor) backDoor.Lock(false);  // allow retreat anytime (or set true if you want it locked during combat)
    }

    public void BeginRoom() { enemyManager?.Activate(); }

    void OnAllDead()
    {
        if (exitDoor) exitDoor.Lock(false);
        // if you want backtracking only after clear, also unlock backDoor here:
        // if (backDoor) backDoor.Lock(false);
    }

    public void UseExit() { sequencer?.NextRoom(); }
    public void UseBack() { sequencer?.PrevRoom(); }
}