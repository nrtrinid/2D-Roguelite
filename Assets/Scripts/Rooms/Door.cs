using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    public enum DoorType { Forward, Backward }
    public DoorType type = DoorType.Forward;

    public Room room;
    public GameObject lockedVisual;
    public GameObject openVisual;
    public bool locked = true;

    void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    public void Lock(bool state)
    {
        locked = state;
        if (lockedVisual) lockedVisual.SetActive(state);
        if (openVisual) openVisual.SetActive(!state);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (locked) return;
        if (!other.CompareTag("Player")) return;

        if (type == DoorType.Forward) room.UseExit();
        else                          room.UseBack();
    }
}