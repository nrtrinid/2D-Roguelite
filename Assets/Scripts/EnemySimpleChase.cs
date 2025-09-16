using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChasePlus : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 3f;
    public float acceleration = 20f;
    public float stopDistance = 1.2f;
    [Range(0f, 1f)] public float brakingFactorNearStop = 0.5f; // slow down smoothly near stopDistance

    [Header("Targeting")]
    public float detectionRadius = 7f;
    public LayerMask playerMask;      // should include Player
    public LayerMask losBlockers;     // walls/obstacles that block line-of-sight
    public float forgetTargetAfter = 2.0f; // seconds without LoS before forgetting

    [Header("Avoidance")]
    public LayerMask obstacleMask;    // usually same as losBlockers
    public float feelerLength = 1.2f; // forward ray length
    [Range(5f, 60f)] public float feelerAngle = 30f; // degrees left/right for side feelers
    public float avoidForce = 2.5f;   // scales the avoidance steering

    [Header("Separation")]
    public LayerMask enemyMask;       // enemies (including self’s layer) for separation
    public float separationRadius = 0.8f;
    public float separationForce = 2.0f;
    const int SEP_BUFFER = 16;
    static readonly Collider2D[] sepHits = new Collider2D[SEP_BUFFER];

    Rigidbody2D rb;
    Transform target;
    Vector2 lastKnownPos;
    float lastSeenTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void FixedUpdate()
    {
        AcquireOrValidateTarget();

        // Desired velocity from chase logic
        Vector2 desired = Vector2.zero;
        Vector2 toTarget = Vector2.zero;
        float dist = 0f;

        if (target != null)
        {
            toTarget = (Vector2)target.position - rb.position;
            dist = toTarget.magnitude;

            // Smooth braking when near stop distance
            if (dist > stopDistance)
            {
                float brake = 1f;
                if (dist < stopDistance * 2f) // start braking within 2x stop distance
                    brake = Mathf.Lerp(brakingFactorNearStop, 1f, Mathf.InverseLerp(stopDistance, stopDistance * 2f, dist));

                desired = toTarget.normalized * (speed * brake);
                lastKnownPos = target.position;
            }
        }
        else
        {
            // Move toward last known position for a short time (search behavior)
            toTarget = lastKnownPos - rb.position;
            dist = toTarget.magnitude;
            if (dist > stopDistance)
                desired = toTarget.normalized * (speed * 0.8f); // a bit slower when searching
        }

        // Add avoidance (feeler rays)
        desired += ComputeAvoidance() * avoidForce;

        // Add separation from nearby enemies
        desired += ComputeSeparation() * separationForce;

        // Accelerate toward desired
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desired, acceleration * Time.fixedDeltaTime);

        // Face movement (optional)
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
            transform.right = rb.linearVelocity.normalized;
    }

    void AcquireOrValidateTarget()
    {
        // If no target, search in radius
        if (!target)
        {
            var hit = Physics2D.OverlapCircle(rb.position, detectionRadius, playerMask);
            if (hit) { target = hit.transform; lastKnownPos = target.position; lastSeenTimer = 0f; }
            return;
        }

        // If we have a target, check line of sight
        Vector2 from = rb.position;
        Vector2 to = target.position;
        Vector2 dir = (to - from).normalized;
        float dist = Vector2.Distance(from, to);

        // If something blocks LoS, tick the timer; else reset it
        var hitInfo = Physics2D.Raycast(from, dir, dist, losBlockers);
        bool sees = !hitInfo;
        if (sees)
        {
            lastSeenTimer = 0f;
            lastKnownPos = to;
        }
        else
        {
            lastSeenTimer += Time.fixedDeltaTime;
            if (lastSeenTimer >= forgetTargetAfter)
            {
                target = null; // will fall back to lastKnownPos search movement
            }
        }
    }

    Vector2 ComputeAvoidance()
    {
        // Use current velocity direction as "forward"; if nearly zero, point toward last known target
        Vector2 fwd = rb.linearVelocity.sqrMagnitude > 0.001f
            ? rb.linearVelocity.normalized
            : (lastKnownPos - rb.position).normalized;

        if (fwd.sqrMagnitude < 0.0001f) return Vector2.zero;

        Vector2 avoid = Vector2.zero;

        // Forward feeler
        avoid += Feeler(fwd);

        // Angled feelers
        Vector2 leftDir = Rotate(fwd, feelerAngle * Mathf.Deg2Rad);
        Vector2 rightDir = Rotate(fwd, -feelerAngle * Mathf.Deg2Rad);
        avoid += Feeler(leftDir);
        avoid += Feeler(rightDir);

        return avoid;
    }

    Vector2 Feeler(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return Vector2.zero;
        var hit = Physics2D.Raycast(rb.position, dir, feelerLength, obstacleMask);
        if (!hit) return Vector2.zero;

        // Steer away from obstacle surface normal; stronger when very close
        float proximity = 1f - Mathf.Clamp01(hit.distance / feelerLength);
        return hit.normal * (proximity * proximity); // quadratic falloff feels nicer
    }

    Vector2 ComputeSeparation()
    {
        var filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.SetLayerMask(enemyMask);

        int count = Physics2D.OverlapCircle(rb.position, separationRadius, filter, sepHits);
        
        if (count <= 0) return Vector2.zero;

        Vector2 push = Vector2.zero;
        int used = 0;

        for (int i = 0; i < count; i++)
        {
            var c = sepHits[i];
            if (!c || c.attachedRigidbody == rb) continue; // ignore self
            Vector2 toMe = (Vector2)rb.position - (Vector2)c.transform.position;
            float d = toMe.magnitude + 0.0001f;
            // inverse-square push
            push += toMe / (d * d);
            used++;
        }

        if (used == 0) return Vector2.zero;
        return push / used; // average push
    }

    static Vector2 Rotate(Vector2 v, float radians)
    {
        float s = Mathf.Sin(radians);
        float c = Mathf.Cos(radians);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)rb.position : transform.position, detectionRadius);

        // feelers
        if (Application.isPlaying)
        {
            Vector2 fwd = rb.linearVelocity.sqrMagnitude > 0.001f ? rb.linearVelocity.normalized : transform.right;
            Vector3 pos = rb.position;
            Vector2 left = Rotate(fwd, feelerAngle * Mathf.Deg2Rad);
            Vector2 right = Rotate(fwd, -feelerAngle * Mathf.Deg2Rad);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pos, pos + (Vector3)(fwd * feelerLength));
            Gizmos.DrawLine(pos, pos + (Vector3)(left * feelerLength * 0.9f));
            Gizmos.DrawLine(pos, pos + (Vector3)(right * feelerLength * 0.9f));
        }
    }
#endif
}