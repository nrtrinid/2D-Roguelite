using System.Collections;
using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform muzzle;
    public SimplePool bulletPool;
    public SpriteRenderer weaponSpriteRenderer; // optional

    [Header("Data")]
    public WeaponDefinition def;

    [Header("Auto Fire (runtime)")]
    [Tooltip("Runtime state: can be toggled if the current weapon allows auto.")]
    public bool autoEnabled = false;

    float cd;                 // cooldown timer
    bool triggerHeld;         // input state
    GameObject ownerRef;      // remember owner during hold/burst
    bool burstRunning;        // prevent stacking bursts

    void Awake()
    {
        if (def && bulletPool && def.bulletPrefab && bulletPool.prefab != def.bulletPrefab)
            bulletPool.prefab = def.bulletPrefab;

        // Seed runtime auto from weapon defaults
        if (def) autoEnabled = def.allowAuto && def.autoOnByDefault;

        ApplyWeaponVisual();
    }

    void Update()
    {
        if (cd > 0f) cd -= Time.deltaTime;

        // Auto only if the weapon allows it
        if (def && (triggerHeld || (autoEnabled && def.allowAuto)) && cd <= 0f)
        {
            FireCycle();   // one cycle per cooldown
            cd = def.fireCooldown;
        }
    }

    public void ToggleAuto()
    {
        if (def && def.allowAuto) autoEnabled = !autoEnabled;
        else autoEnabled = false; // safety if weapon doesn't allow auto
        // Optional: SFX/UI feedback here
    }

    public void SetWeapon(WeaponDefinition newDef)
    {
        def = newDef;
        if (bulletPool && def && def.bulletPrefab) bulletPool.prefab = def.bulletPrefab;
        ApplyWeaponVisual();
        cd = 0f;
        burstRunning = false;

        // Re-seed runtime auto from new weapon defaults
        autoEnabled = def && def.allowAuto && def.autoOnByDefault;
    }

    void ApplyWeaponVisual()
    {
        if (weaponSpriteRenderer && def && def.weaponSprite)
            weaponSpriteRenderer.sprite = def.weaponSprite;
    }

    // Input hooks from PlayerController
    public void OnFirePressed(Vector2 _, GameObject owner)
    {
        ownerRef = owner;
        triggerHeld = true;

        // Immediate cycle if ready (snappy feel)
        if (def != null && cd <= 0f)
        {
            FireCycle();
            cd = def.fireCooldown;
        }
    }

    public void OnFireReleased()
    {
        triggerHeld = false;
    }

    // --- Core: one action per cooldown depending on mode ---
    void FireCycle()
    {
        if (def == null || muzzle == null) return;

        Vector2 dir = (Vector2)muzzle.right;

        switch (def.fireMode)
        {
            case FireMode.Semi:
                FireSingle(dir, ownerRef ? ownerRef : gameObject);
                break;

            case FireMode.Burst:
                if (!burstRunning)
                    StartCoroutine(FireBurst(ownerRef ? ownerRef : gameObject));
                break;

            case FireMode.Shotgun:
                FireShotgun(dir, ownerRef ? ownerRef : gameObject);
                break;
        }
    }

    IEnumerator FireBurst(GameObject owner)
    {
        burstRunning = true;
        var cols = owner.GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < def.burstCount; i++)
        {
            Vector2 dir = (Vector2)muzzle.right; // live aim each shot
            SpawnBullet(ApplySpread(dir, def.spreadDegrees), cols);

            if (i < def.burstCount - 1)
                yield return new WaitForSeconds(def.burstInterval);
        }

        burstRunning = false;
    }

    void FireShotgun(Vector2 dir, GameObject owner)
    {
        var cols = owner.GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < def.pellets; i++)
            SpawnBullet(ApplySpread(dir, def.spreadDegrees), cols);
    }

    void FireSingle(Vector2 dir, GameObject owner)
    {
        var cols = owner.GetComponentsInChildren<Collider2D>();
        SpawnBullet(dir, cols);
    }

    void SpawnBullet(Vector2 dir, Collider2D[] ownerCols)
    {
        if (!bulletPool || !def) return;

        var go = bulletPool.Get();
        go.transform.position = muzzle.position;
        go.transform.rotation = Quaternion.identity;

        var b = go.GetComponent<Bullet>();
        b.Configure(def.damage, def.bulletSpeed, def.bulletLife, def.hitMask);
        b.Init(dir, bulletPool, ownerCols);
    }

    static Vector2 ApplySpread(Vector2 dir, float spreadDeg)
    {
        if (spreadDeg <= 0f) return dir.normalized;
        float half = spreadDeg * 0.5f;
        float angle = Random.Range(-half, half);
        return ((Vector2)(Quaternion.Euler(0, 0, angle) * (Vector3)dir)).normalized;
    }
}
