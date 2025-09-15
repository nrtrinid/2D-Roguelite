using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 3;
    [SerializeField] int hp;

    [Header("Death Behavior")]
    public bool destroyOnDeath = true;   // <- NEW: set FALSE on the Player

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onDeath;

    void Awake() { hp = maxHP; }
    public int Current => hp;

    bool isDead;
public void TakeDamage(int amount) 
{
    if (amount <= 0 || isDead) return;

    hp -= amount;
    onDamaged?.Invoke();

    // Trigger hit-stop only if NOT the player
    if (!CompareTag("Player"))
    {
        HitStop.I?.DoStop(0.03f, 0f);
    }

    if (hp <= 0)
    {
        isDead = true;
        onDeath?.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
    }
}

public void ResetHP() { hp = maxHP; isDead = false; }

}
