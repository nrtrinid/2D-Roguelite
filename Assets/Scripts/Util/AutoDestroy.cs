using UnityEngine;
public class AutoDestroy : MonoBehaviour
{
    public float life = 0.2f;
    void OnEnable() { Destroy(gameObject, life); }
}