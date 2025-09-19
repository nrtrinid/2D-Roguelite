using UnityEngine;
using UnityEngine.SceneManagement;

public class HubUI : MonoBehaviour
{
    public string runSceneName = "Run_Main";
    public void StartRun() => SceneManager.LoadScene(runSceneName);
    public void QuitGame() => Application.Quit();
}
