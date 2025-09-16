using UnityEngine;
using UnityEngine.Events;
using System;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 3;
    [SerializeField] int hp;

    [Header("Death Behavior")]
    public bool destroyOnDeath = true;   // set FALSE on Player

    [Header("Events (Inspector)")]
    public UnityEvent onDamaged;         // no-arg, for VFX/SFX via Inspector
    public UnityEvent onDeath;           // no-arg, for VFX/SFX via Inspector

    // >>> NEW: code-facing events with a Health parameter (nice for managers)
    public event Action<Health> Hurt;
    public event Action<Health> Died;
    public event Action<Health> Healed;

    public int Current => hp;
    public bool IsDead => isDead;

    bool isDead;

    void Awake() { hp = maxHP; }

    // >>> NEW: canonical public API
    public void Damage(int amount) => TakeDamage(amount);
    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;
        int before = hp;
        hp = Mathf.Min(maxHP, hp + amount);
        if (hp > before)
        {
            Healed?.Invoke(this);
            // (optional) you could add a UnityEvent onHealed if you want Inspector hooks
        }
    }

    // Keep your original method working
    public void TakeDamage(int amount) 
    {
        if (amount <= 0 || isDead) return;

        hp -= amount;
        onDamaged?.Invoke();
        Hurt?.Invoke(this);

        // Hit-stop only if NOT the player
        if (!CompareTag("Player"))
            HitStop.I?.DoStop(0.03f, 0f);

        if (hp <= 0)
        {
            isDead = true;
            onDeath?.Invoke();
            Died?.Invoke(this);
            if (destroyOnDeath) Destroy(gameObject);
        }
    }

    public void ResetHP()
    {
        hp = maxHP;
        isDead = false;
    }

    // >>> Optional helpers
    public void SetMax(int newMax, bool refill = true)
    {
        maxHP = Mathf.Max(1, newMax);
        if (refill) hp = maxHP;
        hp = Mathf.Min(hp, maxHP);
    }
}