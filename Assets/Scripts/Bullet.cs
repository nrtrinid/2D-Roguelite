using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 18f;
    public int damage = 1;
    public float life = 1.2f;
    public LayerMask hitMask;

    Rigidbody2D rb;
    float t;
    SimplePool pool;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;              // no drop in 2D
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Init(Vector2 dir, SimplePool p)
    {
        pool = p;
        t = 0f;
        rb.linearVelocity = -dir.normalized * speed;
        transform.right = dir.normalized;       // orient sprite
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
        rb.linearVelocity = Vector2.zero; // reset for pooling
        pool.ReturnToPool(gameObject);
    }
}
