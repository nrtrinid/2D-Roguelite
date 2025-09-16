using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine; // requires com.unity.cinemachine

public enum TravelDir { Forward, Backward }

public class RoomSequencer : MonoBehaviour
{
    [Header("Sequence")]
    [Tooltip("Ordered list of room prefabs (root must have Room.cs).")]
    public List<GameObject> roomPrefabs = new List<GameObject>();

    [Tooltip("Parent under which rooms are instantiated at runtime.")]
    public Transform roomsRoot;

    [Tooltip("Fallback spawn in scene if a room has no playerEntrance/Back.")]
    public Transform playerSpawn;

    [Tooltip("If no Player is found, this prefab will be spawned.")]
    public GameObject playerPrefab;

    [Header("Transition")]
    [Tooltip("If true, uses the coroutine path (hooks for screen fade).")]
    public bool useFadeTransition = false;

    // runtime state
    GameObject currentRoomGO;
    Room currentRoom;
    Transform player;          // cached player transform
    int roomIndex;             // local index if Game.I isn't present
    TravelDir lastDir = TravelDir.Forward;

    void Start()
    {
        // Ensure a rooms parent exists
        if (!roomsRoot)
        {
            var go = new GameObject("Rooms");
            roomsRoot = go.transform;
        }

        // Cache (or create) player
        var found = GameObject.FindWithTag("Player");
        if (found) player = found.transform;
        else if (playerPrefab)
        {
            Vector3 pos = playerSpawn ? playerSpawn.position : Vector3.zero;
            player = Instantiate(playerPrefab, pos, Quaternion.identity).transform;
            player.gameObject.tag = "Player";
        }

        // Resolve starting index
        int desiredIndex = (Game.I != null) ? Game.I.CurrentRun.roomIndex : 0;
        desiredIndex = Mathf.Clamp(desiredIndex, 0, Mathf.Max(0, roomPrefabs.Count - 1));

        LoadRoom(desiredIndex, TravelDir.Forward);  // first entry treated as forward
    }

    // ----- Public API -----

    public void ReloadRoom()
    {
        LoadRoom(GetCurrentIndex(), lastDir);
    }

    public void NextRoom()
    {
        int next = GetCurrentIndex() + 1;
        if (next >= roomPrefabs.Count)
        {
            // End of run behavior
            if (Game.I != null) { Game.I.CurrentRun.victory = true; Game.I.ReturnToTitle(); }
            else                 { LoadRoom(0, TravelDir.Forward); }
            return;
        }
        LoadRoom(next, TravelDir.Forward);
    }

    public void PrevRoom()
    {
        int prev = GetCurrentIndex() - 1;
        if (prev < 0) return; // already at first room
        LoadRoom(prev, TravelDir.Backward);
    }

    public void LoadRoom(int index) => LoadRoom(index, TravelDir.Forward);

    public void LoadRoom(int index, TravelDir dir)
    {
        // Guards
        if (roomPrefabs == null || roomPrefabs.Count == 0)
        {
            Debug.LogError("[RoomSequencer] No roomPrefabs assigned.");
            return;
        }

        index = Mathf.Clamp(index, 0, roomPrefabs.Count - 1);
        SetCurrentIndex(index);
        lastDir = dir;

        if (useFadeTransition)
        {
            StartCoroutine(LoadRoomCo(index, dir));
            return;
        }

        InstantiateAndPlace(index, dir);
    }

    // ----- Internals -----

    IEnumerator LoadRoomCo(int index, TravelDir dir)
    {
        // TODO: yield return ScreenFader.Instance.FadeOut();
        InstantiateAndPlace(index, dir);
        // TODO: yield return ScreenFader.Instance.FadeIn();
        yield return null;
    }

    void InstantiateAndPlace(int index, TravelDir dir)
    {
        // Destroy previous room
        if (currentRoomGO) Destroy(currentRoomGO);

        // Validate prefab slot
        var prefab = roomPrefabs[index];
        if (!prefab)
        {
            Debug.LogError($"[RoomSequencer] roomPrefabs[{index}] is null.");
            return;
        }

        // Instantiate
        currentRoomGO = Instantiate(prefab, roomsRoot);
        currentRoom   = currentRoomGO.GetComponent<Room>();
        if (!currentRoom)
        {
            Debug.LogError($"[RoomSequencer] Prefab '{prefab.name}' has no Room component on root.");
            return;
        }

        // Wire back-reference and start
        currentRoom.sequencer = this;
        currentRoom.BeginRoom();

        // Place player at correct entrance
        if (player)
        {
            Vector3 spawn =
                (dir == TravelDir.Forward  && currentRoom.playerEntrance)     ? currentRoom.playerEntrance.position :
                (dir == TravelDir.Backward && currentRoom.playerEntranceBack) ? currentRoom.playerEntranceBack.position :
                (playerSpawn ? playerSpawn.position : player.position);

            player.position = spawn;
        }

        // Update Cinemachine confiner to this room's bounds
        var vcam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null && currentRoom.confinerBounds != null)
        {
            var conf = vcam.GetComponent<CinemachineConfiner2D>();
            if (conf != null) conf.BoundingShape2D = currentRoom.confinerBounds;
        }
    }

    int GetCurrentIndex() => (Game.I != null) ? Game.I.CurrentRun.roomIndex : roomIndex;
    void SetCurrentIndex(int idx)
    {
        if (Game.I != null) Game.I.CurrentRun.roomIndex = idx;
        roomIndex = idx;
    }
}