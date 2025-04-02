using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    [SerializeField] private Image gameDone;

    [Header("Buttons")]

    public Button resumeButton;
    public Button gameDoneButton;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        InitializeUI();
        gameDoneButton.onClick.AddListener(OnClickGameDoneButton);
    }

    private void InitializeUI() // UI 정보 초기화
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        if (gameDone != null)
        {
            gameDone.gameObject.SetActive(false);
        }
    }

    public void TogglePause()
    {
        bool isActive = !pauseMenu.activeSelf;
        pauseMenu.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1;
    }

    public void IsGameOver()
    {
        bool isGameOver = !gameOverMenu.activeSelf;
        gameOverMenu.SetActive(isGameOver);
        Time.timeScale = isGameOver ? 0 : 1;
    }

    public void ShowGameDone()
    {
        gameDone.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnClickGameDoneButton()
    {
        SceneManager.LoadScene("MainMenu");   
    }
}
