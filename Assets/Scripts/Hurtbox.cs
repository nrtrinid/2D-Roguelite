using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int damage = 1;
    public float tick = 0.4f;   // seconds between hits
    float nextHitTime;

    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < nextHitTime) return;
        if (other.TryGetComponent(out Health hp))
        {
            hp.TakeDamage(damage);
            nextHitTime = Time.time + tick;
        }
    }
}
