using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // Drag the Player here in Inspector
    public float smoothSpeed = 0.125f;
    public Vector3 offset;     // Optional: e.g. (0, 0, -10) for 2D

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
