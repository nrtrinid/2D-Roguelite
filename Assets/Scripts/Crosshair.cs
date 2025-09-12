using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairFollow : MonoBehaviour
{
    public Transform player;           // assign Player
    public Transform weaponPivot;      // same pivot you rotate
    public float stickDistance = 3.5f; // radius for gamepad aim
    public float stickDeadzone = 0.15f;

    PlayerInput pInput;
    Camera cam;
    Vector2 lastNonZeroStick = Vector2.right;

    void Awake()
    {
        pInput = FindFirstObjectByType<PlayerInput>();
        cam = Camera.main;
        Cursor.visible = false; // optional
    }

    void LateUpdate()
    {
        var actions = pInput.actions;
        Vector2 aimRaw = actions["Aim"].ReadValue<Vector2>();

        // Mouse position path -> big values (pixels). Stick -> small (?1..1).
        bool looksLikeMouse = aimRaw.magnitude > 2f && cam != null;

        if (looksLikeMouse)
        {
            // distance from camera to world plane (e.g., camera at z = -10 -> distance = 10)
            float z = -cam.transform.position.z;
            Vector3 world = cam.ScreenToWorldPoint(new Vector3(aimRaw.x, aimRaw.y, z));
            world.z = 0f;
            transform.position = world;
        }
        else
        {
            if (aimRaw.sqrMagnitude > stickDeadzone * stickDeadzone)
                lastNonZeroStick = aimRaw.normalized;

            Vector3 basePos = (player ? player.position : weaponPivot.position);
            transform.position = basePos + (Vector3)(lastNonZeroStick * stickDistance);
        }
    }
}
