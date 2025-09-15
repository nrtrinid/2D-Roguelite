using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop I;
    void Awake() { I = this; }

    bool active;
    float timer, storedScale;

    public void DoStop(float duration = 0.06f, float timescale = 0f)
    {
        if (active) return;
        active = true;
        timer = duration;
        storedScale = Time.timeScale;
        Time.timeScale = timescale;
    }

    void Update()
    {
        if (!active) return;
        timer -= Time.unscaledDeltaTime;
        if (timer <= 0f)
        {
            Time.timeScale = storedScale;
            active = false;
        }
    }
}
