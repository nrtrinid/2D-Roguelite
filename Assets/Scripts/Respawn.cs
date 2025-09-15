using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerRespawn : MonoBehaviour
{
    public Transform spawnPoint;
    public float invulnSeconds = 0.8f; // optional: give the player brief i-frames

    Health health;
    Rigidbody2D rb;

    void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Hook this in the Inspector to Health.onDeath
    public void RespawnNow()
    {
        // reset motion
        if (rb) rb.linearVelocity = Vector2.zero;

        // move to spawn
        transform.position = spawnPoint ? spawnPoint.position : Vector3.zero;

        // restore HP
        health.ResetHP();

        // simple invulnerability window (optional): disable collisions or damage gate here if you add one later
        // e.g., flip a "canTakeDamage" bool for invulnSeconds via coroutine
        // (You can add this later—keeping it minimal for now.)
    }
}
