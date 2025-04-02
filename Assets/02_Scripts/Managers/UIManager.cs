using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : SingleTon<UIManager>
{
    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    [SerializeField] private Image gameDone;
    //public GameObject inventoryUI;
    //public GameObject statusUI;
    //public GameObject weaponChangeUI;

    //[Header("UI Elements")]
    //public Image weaponIcon;
    //public Image[] itemSlots;
    //public Slider healthBar;
    //public TMP_Text healthText;

    [Header("Buttons")]
    //public Button pauseButton;
    public Button resumeButton;
    public Button gameDoneButton;
    //public Button statusButton;

    


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
