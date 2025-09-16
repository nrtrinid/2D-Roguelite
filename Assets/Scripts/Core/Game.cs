using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game I { get; private set; }
    public RunData CurrentRun { get; private set; } = new RunData();

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewRun(int? seed = null)
    {
        CurrentRun = new RunData();
        CurrentRun.seed = seed ?? Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(CurrentRun.seed);
        SceneManager.LoadScene("Run_Main");
    }

    public void ReturnToTitle() => SceneManager.LoadScene("Title");
    public void Quit() => Application.Quit();
}

[System.Serializable]
public class RunData
{
    public int seed;
    public int roomIndex;
    public int gold;
    public bool victory;
}