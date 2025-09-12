using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    public SimplePool bulletPool;
    public Transform muzzle;
    public float fireCooldown = 0.15f;
    float cd;

    void Update() { if (cd > 0) cd -= Time.deltaTime; }

    public void TryFire(Vector2 dir)
    {
        if (cd > 0) return;
        cd = fireCooldown;

        var go = bulletPool.Get();
        go.transform.position = muzzle.position;
        go.transform.rotation = Quaternion.identity; // bullet sets its own facing

        var b = go.GetComponent<Bullet>();
        b.Init(dir, bulletPool);
    }
}
