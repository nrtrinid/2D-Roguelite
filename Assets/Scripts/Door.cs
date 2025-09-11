using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    public GameObject targetRoomRoot;   // drag the root GameObject of the target room (existing in scene)
    public Transform targetSpawn;      // drag the spawn Transform inside the target room
    public GameObject currentRoomRoot; // drag THIS room’s root (to toggle off), optional
    public bool toggleRooms = true;    // if true, deactivate current room and activate target

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!targetSpawn) { Debug.LogWarning("No targetSpawn set."); return; }

        // Teleport player
        other.transform.position = targetSpawn.position;

        if (toggleRooms)
        {
            if (currentRoomRoot) currentRoomRoot.SetActive(false);
            if (targetRoomRoot)  targetRoomRoot.SetActive(true);
        }
    }
}