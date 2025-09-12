using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Aiming")]
    public Transform weaponPivot;   // parent that rotates
    public Transform firePoint;     // muzzle
    [Range(0f, 1f)] public float stickDeadzone = 0.15f;

    [Header("Crosshair")]           // NEW
    public Transform crosshair;     // assign your Crosshair object in Inspector (authoritative aim)

    [Header("Shooting")]
    public WeaponShooter shooter;

    Rigidbody2D rb;
    PlayerInput pInput;
    Camera cam;

    Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pInput = GetComponent<PlayerInput>();
        cam = Camera.main;
    }

    void Update()
    {
        var actions = pInput.actions;

        // --- movement only ---
        moveInput = actions["Move"].ReadValue<Vector2>();

        // --- AIM: crosshair is the source of truth ---
        if (crosshair && weaponPivot)
        {
            Vector2 aimDir = ((Vector2)crosshair.position - (Vector2)weaponPivot.position).normalized;

            if (aimDir.sqrMagnitude > 0.0001f)
            {
                weaponPivot.right = -aimDir;
            }
        }

        // --- fire ---
        if (actions["Fire"].WasPressedThisFrame() && shooter && firePoint)
        {
            shooter.TryFire(firePoint.right);
        }

        // if (actions["Pause"].WasPressedThisFrame()) { /* PauseMenu.Toggle(); */ }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        rb.SetRotation(0f);
    }
}
