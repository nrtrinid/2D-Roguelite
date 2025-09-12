using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHP = 3;
    public UnityEvent onDeath;
    private int hp;
    void Awake() { hp = maxHP; }
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) { onDeath?.Invoke(); Destroy(gameObject); }
    }
}
