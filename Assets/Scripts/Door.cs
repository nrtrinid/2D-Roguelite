using UnityEngine;
using Unity.Cinemachine;

public class Door : MonoBehaviour
{
    public Transform destination;
    public PolygonCollider2D destinationBounds;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // move player to the destination point
        other.transform.position = destination.position;

        // find the active CinemachineCamera
        var vcam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            var conf = vcam.GetComponent<CinemachineConfiner2D>();
            if (conf != null)
                conf.BoundingShape2D = destinationBounds;
        }
    }
}
