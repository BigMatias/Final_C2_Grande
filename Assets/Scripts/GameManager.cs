using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private PlayerMovement player;

    private bool _gamePaused = false;
    private bool _gameOver = false;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        SetCursorState(locked: true);
        player.onDie += HandleGameOver;
    }

    private void OnDestroy()
    {
        player.onDie -= HandleGameOver;
    }

    private void Update()
    {
        if (_gameOver) return;

        if (Input.GetMouseButtonDown(0) && !_gamePaused)
            SetCursorState(true);
        
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        TogglePause();
    }

    private void HandleGameOver()
    {
        _gameOver = true;
        Time.timeScale = 0f;
        SetCursorState(locked: false);
    }
    
    public void TogglePause()
    {
        _gamePaused = !_gamePaused;

        Time.timeScale = _gamePaused ? 0f : 1f;
        SetCursorState(locked: !_gamePaused);
        pauseMenu.gameObject.SetActive(_gamePaused);
        if (!_gamePaused) optionsMenu.gameObject.SetActive(false);
    }

    private void SetCursorState(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}