using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMeleeDriver : MonoBehaviour
{
    public MeleeWeapon melee;
    public Transform firePoint; // your existing muzzle/forward
    PlayerInput pInput;
    Camera cam;

    void Awake()
    {
        pInput = GetComponent<PlayerInput>();
        cam = Camera.main;
    }

    void Update()
    {
        if (!melee) return;
        var actions = pInput.actions;

        Vector2 aimDir = firePoint ? (Vector2)firePoint.right : Vector2.right;

        if (actions["Fire"].WasPressedThisFrame() && melee.CanAttack())
            melee.TryAttack(aimDir);
    }
}
