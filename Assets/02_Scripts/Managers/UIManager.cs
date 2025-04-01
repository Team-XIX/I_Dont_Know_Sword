using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : SingleTon<UIManager>
{
    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject inventoryUI;
    public GameObject statusUI;
    public GameObject weaponChangeUI;

    [Header("UI Elements")]
    public Image weaponIcon;
    public Image[] itemSlots;
    public Slider healthBar;
    public TMP_Text healthText;

    [Header("Buttons")]
    public Button pauseButton;
    public Button resumeButton;
    public Button statusButton;
   

    private void Start() //Start에서 체력 UI 정보 초기화
    {
        InitializeUI();
    }

    private void InitializeUI() // UI 정보 초기화
    {
        pauseMenu.SetActive(false);
     

        // UpdateHealthUI();
    }

    public void TogglePause() //퍼즈 토글 버튼
    {
        bool isActive = !pauseMenu.activeSelf;
        pauseMenu.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1;
    }

    public void HealthUI()
    {
        // 추가해야함 
        // 오브젝트 풀 적용
    }

}
