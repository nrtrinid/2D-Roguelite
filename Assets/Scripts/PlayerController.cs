using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Aiming")]
    public Transform weaponPivot;   // rotates toward aim
    public Transform firePoint;     // muzzle
    [Range(0f, 1f)] public float stickDeadzone = 0.15f;

    [Header("Crosshair")]
    public Transform crosshair;     // authoritative aim

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

        // --- AIM: crosshair is source of truth ---
        if (crosshair && weaponPivot)
        {
            Vector2 aimDir = ((Vector2)crosshair.position - (Vector2)weaponPivot.position).normalized;
            if (aimDir.sqrMagnitude > 0.0001f)
                weaponPivot.right = aimDir;
        }

        // --- FIRE handling ---
        if (shooter && firePoint)
        {
            Vector2 aimDir = firePoint.right; // muzzle forward

            if (actions["Fire"].WasPressedThisFrame())
            {
                shooter.OnFirePressed(aimDir, gameObject);
            }
            if (actions["Fire"].IsPressed())
            {
                // Auto handled inside WeaponShooter.Update()
                // nothing extra here
            }
            if (actions["Fire"].WasReleasedThisFrame())
            {
                shooter.OnFireReleased();
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        rb.SetRotation(0f);
    }
}