using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public MeleeWeaponDefinition def;

    [Header("Scene Refs")]
    public Transform pivot;        // faces aim; +X is forward
    public Transform origin;       // spawn / arc origin (e.g., FirePoint)
    public Health owner;           // who swung (ignored)
    public Transform sweepVisual;  // optional child that visually sweeps

    [Header("Debug")]
    public bool debug = false;
    public bool drawActiveArc = true;
    public bool logHits = false;

    bool attacking;
    float cooldown;

    [Header("VFX Tuning")]
    public Vector2 spawnOffset = new Vector2(0.5f, 0f);
    public bool attachToSweep = false;

    // dedupe per-entity per swing
    readonly HashSet<Health> hitEntitiesThisSwing = new();

    void Awake()
    {
        if (!origin) origin = transform;
        if (!owner)  owner  = GetComponentInParent<Health>();

        if (debug)
        {
            if (!def)   Debug.LogWarning("[Melee] Missing DEF", this);
            if (!pivot) Debug.LogWarning("[Melee] Missing PIVOT", this);
            if (!origin)Debug.LogWarning("[Melee] Missing ORIGIN", this);
        }
    }

    void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;
    }

    public bool CanAttack() => !attacking && cooldown <= 0f;

    public void TryAttack(Vector2 aimDir)
    {
        if (!CanAttack())
        {
            if (debug) Debug.Log($"[Melee] Blocked (attacking={attacking}, cd={cooldown:0.00})", this);
            return;
        }
        if (aimDir.sqrMagnitude < 0.0001f) aimDir = Vector2.right;
        StartCoroutine(AttackCo(aimDir.normalized));
    }

    IEnumerator AttackCo(Vector2 aimDir)
    {
        attacking = true;
        cooldown  = def.windup + def.activeTime + def.recovery;

        if (pivot) pivot.right = aimDir;

        if (debug) Debug.Log($"[Melee] WINDUP {def.windup:0.00}s → ACTIVE {def.activeTime:0.00}s → REC {def.recovery:0.00}s", this);
        yield return new WaitForSeconds(def.windup);

        // Spawn swing VFX oriented to aim (+X forward)
        if (def.swingVfxPrefab && origin)
        {
            // basis from aim
            Vector2 f = aimDir.normalized;             // forward (+X)
            Vector2 n = new Vector2(-f.y, f.x);        // right/normal (+Y around Z)

            Vector3 pos = origin.position
                        + (Vector3)(f * spawnOffset.x)   // forward offset
                        + (Vector3)(n * spawnOffset.y);  // side offset

            var rot = Quaternion.FromToRotation(Vector3.right, (Vector3)f);
            var go  = Instantiate(def.swingVfxPrefab, pos, rot);

            if (attachToSweep && sweepVisual) go.transform.SetParent(sweepVisual, worldPositionStays:true);

            if (def.swingSfx) AudioSource.PlayClipAtPoint(def.swingSfx, pos, 0.9f);
        }

        hitEntitiesThisSwing.Clear();

        // visual sweep setup
        float startZ = -def.arcDegrees * 0.5f;
        float endZ   =  def.arcDegrees * 0.5f;
        if (sweepVisual) sweepVisual.localRotation = Quaternion.Euler(0, 0, startZ);

        float t = 0f;
        int totalHits = 0;

        while (t < def.activeTime)
        {
            // advance visual sweep
            if (sweepVisual)
            {
                float p = Mathf.Clamp01(t / def.activeTime);
                float z = Mathf.Lerp(startZ, endZ, p);
                sweepVisual.localRotation = Quaternion.Euler(0, 0, z);
            }

            totalHits += DoArcHit(aimDir);

            if (debug && drawActiveArc)
                DrawArcGizmoRuntime(aimDir, Color.cyan, 12, def.range);

            t += Time.deltaTime;
            yield return null;
        }

        if (sweepVisual) sweepVisual.localRotation = Quaternion.identity;

        if (debug && totalHits == 0)
            Debug.Log("[Melee] Active window ended — no hits registered", this);

        yield return new WaitForSeconds(def.recovery);
        attacking = false;
    }

    int DoArcHit(Vector2 aimDir)
    {
        var hits = Physics2D.OverlapCircleAll(origin.position, def.range, def.hitMask);
        float halfArc = def.arcDegrees * 0.5f;

        if (debug && logHits)
            Debug.Log($"[Melee] Overlaps={hits.Length}  range={def.range}  mask={def.hitMask.value}", this);

        int count = 0;

        foreach (var c in hits)
        {
            // vector from origin to closest point on collider
            Vector2 to = (Vector2)c.bounds.ClosestPoint(origin.position) - (Vector2)origin.position;
            float ang = Vector2.SignedAngle(aimDir, to);
            bool inArc = Mathf.Abs(ang) <= halfArc;

            if (!inArc)
            {
                if (debug && logHits)
                    Debug.DrawLine(origin.position, (Vector2)origin.position + to.normalized * Mathf.Min(def.range, to.magnitude), Color.gray, 0.1f);
                continue;
            }

            // find entity & dedupe by Health
            var h = c.GetComponentInParent<Health>() ?? c.GetComponent<Health>();
            if (!h) { if (debug && logHits) Debug.Log($"[Melee] In arc but no Health on {c.name}", this); continue; }
            if (owner && h == owner) continue;
            if (hitEntitiesThisSwing.Contains(h)) continue;

            // apply damage
            h.Damage(def.damage);
            hitEntitiesThisSwing.Add(h);
            count++;

            // hit VFX/SFX at contact
            Vector2 contact = (Vector2)c.bounds.ClosestPoint(origin.position);
            if (def.hitVfxPrefab)
            {
                var go = Instantiate(def.hitVfxPrefab, contact, Quaternion.identity);
                if (debug && logHits) Debug.Log($"[MeleeVFX] Hit spark {go.name} @ {contact}", this);
            }
            if (def.hitSfx) AudioSource.PlayClipAtPoint(def.hitSfx, contact, 1.0f);

            
            if (debug && logHits)
            {
                Debug.Log($"[Melee] HIT {h.name}  dist={to.magnitude:0.00}  ang={ang:0.0}°", this);
                Debug.DrawLine(origin.position, (Vector2)origin.position + to.normalized * Mathf.Min(def.range, to.magnitude), Color.green, 0.15f);
            }

            // knockback if RB exists
            var rb = h.GetComponent<Rigidbody2D>();
            if (rb) rb.AddForce(to.normalized * def.knockback, ForceMode2D.Impulse);
        }

        return count;
    }

    void DrawArcGizmoRuntime(Vector2 aimDir, Color color, int segments, float radius)
    {
        Vector3 o = origin.position;
        float start = -def.arcDegrees * 0.5f;
        float step  = def.arcDegrees / Mathf.Max(1, segments);

        Vector3 prev = o + (Vector3)(Quaternion.Euler(0,0,start) * (Vector3)aimDir).normalized * radius;
        for (int i = 1; i <= segments; i++)
        {
            float a = start + step * i;
            Vector3 next = o + (Vector3)(Quaternion.Euler(0,0,a) * (Vector3)aimDir).normalized * radius;
            Debug.DrawLine(prev, next, color, 0f);
            prev = next;
        }
        Debug.DrawLine(o, o + (Vector3)aimDir.normalized * radius, color, 0f); // center ray
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!origin) origin = transform;
        float r = def ? def.range : 1.5f;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(origin.position, r);

        if (def && pivot)
        {
            Vector3 f = pivot.right;
            Vector3 L = Quaternion.Euler(0,0,  def.arcDegrees * 0.5f) * f;
            Vector3 R = Quaternion.Euler(0,0, -def.arcDegrees * 0.5f) * f;
            Gizmos.DrawLine(origin.position, origin.position + L.normalized * r);
            Gizmos.DrawLine(origin.position, origin.position + R.normalized * r);
        }
    }
#endif
}