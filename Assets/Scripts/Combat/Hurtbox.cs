using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageOnTouch : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;
    public float tick = 0.4f;              // per-target cooldown

    [Header("Filtering")]
    public LayerMask targetMask;           // set to Player layer(s) in Inspector
    public Health owner;                   // auto-filled from parent

    // per-target cooldowns
    readonly Dictionary<Health, float> nextHitByTarget = new();

    Collider2D myCol;

    void Awake()
    {
        myCol = GetComponent<Collider2D>();
        if (!myCol.isTrigger) myCol.isTrigger = true;

        if (!owner) owner = GetComponentInParent<Health>();

        // Optional: proactively ignore physical collisions with our own colliders
        if (owner)
        {
            var ownerCols = owner.GetComponentsInChildren<Collider2D>(true);
            foreach (var c in ownerCols)
            {
                if (c && c != myCol)
                    Physics2D.IgnoreCollision(myCol, c, true);
            }
        }
    }

    void OnDisable() => nextHitByTarget.Clear();

    void OnTriggerStay2D(Collider2D other)
    {
        // Layer filter
        if ((targetMask.value & (1 << other.gameObject.layer)) == 0) return;

        // Find Health on target (child or parent)
        var hp = other.GetComponent<Health>() ?? other.GetComponentInParent<Health>();
        if (!hp) return;

        // Don't hit ourselves
        if (owner && hp == owner) return;

        // Per-target tick
        if (nextHitByTarget.TryGetValue(hp, out var nextTime) && Time.time < nextTime) return;

        hp.Damage(damage);
        nextHitByTarget[hp] = Time.time + tick;
    }
}