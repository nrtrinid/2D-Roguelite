using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    public float speed = 3f;
    public float acceleration = 20f;
    public float detectionRadius = 7f;
    public float stopDistance = 1.2f;
    public LayerMask playerMask;           // include Player layer

    Rigidbody2D rb;
    Transform target;

    void Awake() { rb = GetComponent<Rigidbody2D>(); rb.gravityScale = 0f; }

    void FixedUpdate()
    {
        if (!target)
        {
            var hit = Physics2D.OverlapCircle(rb.position, detectionRadius, playerMask);
            if (hit) target = hit.transform;
        }

        Vector2 desired = Vector2.zero;
        if (target)
        {
            Vector2 to = (Vector2)target.position - rb.position;
            float dist = to.magnitude;
            if (dist > stopDistance) desired = to.normalized * speed;
        }

        // Smooth velocity change so corners feel good
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desired, acceleration * Time.fixedDeltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}