using UnityEngine;

[CreateAssetMenu(menuName="Roguelite/Weapons/Melee Weapon", fileName="MeleeWeapon")]
public class MeleeWeaponDefinition : ScriptableObject
{
    [Header("Tuning")]
    [Min(1)] public int damage = 2;
    [Range(30f, 200f)] public float arcDegrees = 110f;
    [Min(0.2f)] public float range = 1.6f;
    [Min(0f)] public float windup = 0.08f;
    [Min(0.02f)] public float activeTime = 0.12f;
    [Min(0f)] public float recovery = 0.12f;
    public LayerMask hitMask;
    public float knockback = 4f;

    [Header("FX (optional)")]
    public GameObject swingVfxPrefab;
    public GameObject hitVfxPrefab;
    public AudioClip swingSfx;
    public AudioClip hitSfx;
}
