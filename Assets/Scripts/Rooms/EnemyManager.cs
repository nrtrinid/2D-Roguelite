using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public event Action onAllDead;
    readonly List<Health> alive = new();

    public void Register(Health h)
    {
        if (alive.Contains(h)) return;
        alive.Add(h);
        h.Died += OnEnemyDeath;
    }

    public void Activate()
    {
        foreach (var h in GetComponentsInChildren<Health>(true))
            if (h.CompareTag("Enemy")) Register(h);
        
        if (alive.Count == 0) onAllDead?.Invoke(); 
    }

    void OnEnemyDeath(Health h)
    {
        if (h == null) return;
        h.Died -= OnEnemyDeath;
        alive.Remove(h);
        if (alive.Count == 0) onAllDead?.Invoke();
    }
}
