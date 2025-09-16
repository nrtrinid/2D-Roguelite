using UnityEngine;

public enum FireMode { Semi, Burst, Shotgun }

[CreateAssetMenu(menuName = "Roguelite/Weapon Definition", fileName = "NewWeapon")]
public class WeaponDefinition : ScriptableObject
{
    [Header("Meta")]
    public string displayName = "Weapon";
    public Sprite weaponSprite;

    [Header("Projectile")]
    [Tooltip("Bullet prefab with Bullet.cs + Rigidbody2D + Trigger Collider.")]
    public GameObject bulletPrefab;
    [Min(0.01f)] public float bulletSpeed = 18f;
    [Min(0)]     public int   damage = 1;
    [Min(0.01f)] public float bulletLife = 1.2f;
    [Tooltip("Layers the bullet can hit (e.g., Enemy | World).")]
    public LayerMask hitMask;

    [Header("Firing (common)")]
    public FireMode fireMode = FireMode.Semi;
    [Tooltip("Delay between shots/volleys. For Burst, this is time between bursts.")]
    [Min(0.01f)] public float fireCooldown = 0.15f;

    [Header("Auto Fire (per-weapon)")]
    [Tooltip("If true, this weapon supports auto-fire toggling in WeaponShooter.")]
    public bool allowAuto = true;
    [Tooltip("If true and allowAuto, the weapon starts with auto enabled when equipped.")]
    public bool autoOnByDefault = false;

    // Burst-only
    [HideInInspector] public int   burstCount    = 3;
    [HideInInspector] public float burstInterval = 0.06f;

    // Shotgun-only
    [HideInInspector] public int   pellets       = 6;
    [HideInInspector] public float spreadDegrees = 8f;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(displayName)) displayName = name;
        bulletSpeed   = Mathf.Max(0.01f, bulletSpeed);
        bulletLife    = Mathf.Max(0.01f, bulletLife);
        fireCooldown  = Mathf.Max(0.01f, fireCooldown);
        burstCount    = Mathf.Max(1, burstCount);
        burstInterval = Mathf.Max(0.01f, burstInterval);
        pellets       = Mathf.Max(1, pellets);
        spreadDegrees = Mathf.Clamp(spreadDegrees, 0f, 60f);
    }
#endif
}
