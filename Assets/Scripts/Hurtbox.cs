using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int damage = 1;
    public float tick = 0.4f;
    float nextHitTime;

    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < nextHitTime) return;

        // Look for Health on the collider OR its parents
        var hp = other.GetComponent<Health>();
        if (!hp) hp = other.GetComponentInParent<Health>();   // <- key change

        if (hp)
        {
            hp.TakeDamage(damage);
            nextHitTime = Time.time + tick;
        }
    }
}
