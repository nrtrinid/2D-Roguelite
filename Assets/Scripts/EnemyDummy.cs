using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDummy : MonoBehaviour
{
    public bool idleWiggle = true;
    public float amplitude = 0.05f;
    public float speed = 3f;

    Rigidbody2D rb;
    Vector2 basePos;
    float t;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true; // or Constraints → Freeze Z in Inspector
        basePos = rb.position;
    }

    void FixedUpdate()
    {
        if (!idleWiggle) return;

        t += Time.fixedDeltaTime;
        var target = basePos + Vector2.up * Mathf.Sin(t * speed) * amplitude;

        // Move *through physics*, so collisions with walls are respected
        rb.MovePosition(target);
    }
}
