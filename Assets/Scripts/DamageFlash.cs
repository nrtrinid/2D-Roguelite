using UnityEngine;

public class FlashOnHit : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;   // <- assign child sprite here in Inspector
    public Color flashColor = new(1f, 0.2f, 0.2f);
    public float flashTime = 0.07f;

    Color baseColor;
    float t;

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (sr) baseColor = sr.color;
    }

    public void TriggerFlash()
    {
        if (!sr) return;
        t = flashTime;
        sr.color = flashColor;
    }

    void Update()
    {
        if (t > 0f && sr)
        {
            t -= Time.deltaTime;
            if (t <= 0f) sr.color = baseColor;
        }
    }
}
