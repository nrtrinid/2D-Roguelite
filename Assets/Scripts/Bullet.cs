using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 18f;
    public int damage = 1;
    public float life = 1.2f;
    public LayerMask hitMask;

    Rigidbody2D rb;
    Collider2D col;
    float t;
    SimplePool pool;

    // store who we're ignoring so we can re-enable on Despawn
    Collider2D[] ignoredOwners;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Init(Vector2 dir, SimplePool p, Collider2D[] ignore = null)
    {
        pool = p;
        t = 0f;
        rb.linearVelocity = dir.normalized * speed;
        transform.right = dir.normalized;

        ignoredOwners = ignore;
        if (ignoredOwners != null && ignoredOwners.Length > 0)
            StartCoroutine(TempIgnore(ignoredOwners, 0.1f));
    }

    IEnumerator TempIgnore(Collider2D[] list, float time)
    {
        foreach (var c in list) if (c) Physics2D.IgnoreCollision(col, c, true);
        yield return new WaitForSeconds(time);
        foreach (var c in list) if (c) Physics2D.IgnoreCollision(col, c, false);
        ignoredOwners = null; // we restored them
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t >= life) Despawn();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitMask) == 0) return;

        if (other.TryGetComponent(out Health hp))
            hp.TakeDamage(damage);

        Despawn();
    }

    void Despawn()
    {
        // safety: if we despawn before coroutine restores collisions, restore now
        if (ignoredOwners != null)
        {
            foreach (var c in ignoredOwners) if (c) Physics2D.IgnoreCollision(col, c, false);
            ignoredOwners = null;
        }

        rb.linearVelocity = Vector2.zero;
        pool.ReturnToPool(gameObject);
    }
    public void Configure(int dmg, float spd, float life, LayerMask mask)
    {
        damage = dmg;
        speed = spd;
        this.life = life;
        hitMask = mask;
    }
}
