using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject pauseCanvas;       // drag PauseCanvas here
    public PlayerInput playerInput;      // drag your Player (with PlayerInput) here

    [Header("Scene Names")]
    public string runSceneName = "Run_Main";
    public string hubSceneName = "Hub";

    bool _isPaused;
    InputAction _pauseAction;

    void Awake()
    {
        if (pauseCanvas) pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    void OnEnable()
    {
        if (!playerInput)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput && playerInput.actions != null)
        {
            _pauseAction = playerInput.actions["Pause"];
            if (_pauseAction != null)
            {
                _pauseAction.performed += OnPausePerformed;
                _pauseAction.Enable();
            }
        }
    }

    void OnDisable()
    {
        if (_pauseAction != null)
            _pauseAction.performed -= OnPausePerformed;
    }

    void OnPausePerformed(InputAction.CallbackContext _)
    {
        if (_isPaused) Resume();
        else Pause();
    }

    // Called by the Resume button
    public void OnResume() => Resume();

    // Called by the Restart button
    public void OnRestartRun()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(runSceneName);
    }

    // Called by the Return to Hub button
    public void OnReturnToHub()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(hubSceneName);
    }

    void Pause()
    {
        _isPaused = true;
        if (pauseCanvas) pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Resume()
    {
        _isPaused = false;
        if (pauseCanvas) pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        // set these how you like for gameplay:
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}