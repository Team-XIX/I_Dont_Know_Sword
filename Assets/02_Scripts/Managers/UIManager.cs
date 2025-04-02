using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : SingleTon<UIManager>
{
    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
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
    //public Button statusButton;


    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI() // UI 정보 초기화
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        
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
}
