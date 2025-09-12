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

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        hp -= amount;
        onDamaged?.Invoke();

        if (hp <= 0)
        {
            onDeath?.Invoke();
            if (destroyOnDeath) Destroy(gameObject);   // enemies true, player false
        }
    }

    public void ResetHP() => hp = maxHP;
}
