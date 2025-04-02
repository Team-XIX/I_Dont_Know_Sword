using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;

public class WeaponUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private PlayerWeapon playerWeapon;

    [Header("UI 요소")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponIndexText; // "1/3" 같은 형식

    [Header("무기 아이콘")]
    [SerializeField] private SpriteAtlas weaponIconAtlas; // 무기 아이콘용 스프라이트 아틀라스
    [SerializeField] private Sprite defaultWeaponIcon; // 아이콘이 없을 때 표시할 기본 이미지

    [Header("이미지 크기 설정")]
    [SerializeField] private float fixedHeight = 64f;

    private void Start()
    {
        // WeaponManager 찾기
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }

        // PlayerWeapon 찾기
        if (playerWeapon == null && weaponManager != null)
        {
            playerWeapon = weaponManager.GetComponentInChildren<PlayerWeapon>();
        }

        if (weaponManager == null)
        {
            Debug.LogWarning("WeaponUI: WeaponManager를 찾을 수 없습니다.");
            return;
        }

        // 무기 아이콘 아틀라스 로드 (아직 할당되지 않은 경우)
        if (weaponIconAtlas == null)
        {
            weaponIconAtlas = Resources.Load<SpriteAtlas>("WeaponSprite/WeaponIcons");
            if (weaponIconAtlas == null)
            {
                Debug.LogWarning("WeaponUI: 무기 아이콘 아틀라스를 로드할 수 없습니다.");
            }
        }

        // 무기 변경 이벤트 구독
        weaponManager.OnWeaponChanged += OnWeaponChanged;

        // 초기 무기 UI 업데이트
        WeaponData currentWeapon = weaponManager.GetCurrentWeaponData();
        if (currentWeapon != null)
        {
            UpdateWeaponUI(weaponManager.GetCurrentWeaponIndex(), weaponManager.GetWeaponCount(), currentWeapon);
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (weaponManager != null)
        {
            weaponManager.OnWeaponChanged -= OnWeaponChanged;
        }
    }

    /// <summary>
    /// 무기가 변경되었을 때 호출되는 콜백
    /// </summary>
    private void OnWeaponChanged(int index, int total, WeaponData weaponData)
    {
        UpdateWeaponUI(index, total, weaponData);
    }

    /// <summary>
    /// 무기 UI 업데이트
    /// </summary>
    private void UpdateWeaponUI(int index, int total, WeaponData weaponData)
    {
        // 무기 이름 업데이트
        if (weaponNameText != null && weaponData != null)
        {
            weaponNameText.text = weaponData.name;
        }

        // 무기 인덱스 업데이트 (예: "1/3")
        if (weaponIndexText != null)
        {
            weaponIndexText.text = $"{index + 1}/{total}";
        }

        if (weaponIcon != null && weaponData != null)
        {
            // 아틀라스에서 무기 아이콘 가져오기
            Sprite icon = GetWeaponIconFromAtlas(weaponData.id);
            if (icon != null)
            {
                weaponIcon.sprite = icon;
                weaponIcon.preserveAspect = true;

                // 비율 유지하면서 높이 고정
                float aspectRatio = (float)icon.rect.width / icon.rect.height;
                float newWidth = fixedHeight * aspectRatio;
                weaponIcon.rectTransform.sizeDelta = new Vector2(newWidth, fixedHeight);
            }
            else
            {
                weaponIcon.sprite = defaultWeaponIcon;
                // 기본 아이콘도 같은 높이로 설정
                float aspectRatio = (float)defaultWeaponIcon.rect.width / defaultWeaponIcon.rect.height;
                float newWidth = fixedHeight * aspectRatio;
                weaponIcon.rectTransform.sizeDelta = new Vector2(newWidth, fixedHeight);
            }
        }
    }

    /// <summary>
    /// 무기 ID에 해당하는 아이콘을 아틀라스에서 가져오기
    /// </summary>
    private Sprite GetWeaponIconFromAtlas(int weaponId)
    {
        if (weaponIconAtlas == null) return defaultWeaponIcon;

        // 아이콘 이름 형식을 "weapon_icon_{id}" 또는 "icon_{id}" 등으로 정의
        string[] possibleIconNames = new string[]
        {
            $"weapon_icon_{weaponId}",
            $"icon_{weaponId}",
            $"{weaponId}_icon",
            $"{weaponId}"  // WeaponSprite/Weapon과 동일한 이름 형식을 사용할 경우
        };

        // 가능한 이름 형식으로 아이콘 찾기
        foreach (string iconName in possibleIconNames)
        {
            Sprite icon = weaponIconAtlas.GetSprite(iconName);
            if (icon != null)
            {
                return icon;
            }
        }

        // 찾지 못한 경우 기본 아이콘 반환
        Debug.LogWarning($"WeaponUI: 무기 ID {weaponId}에 대한 아이콘을 찾을 수 없습니다.");
        return defaultWeaponIcon;
    }

    /// <summary>
    /// 다음 무기로 전환 버튼 이벤트 (UI 버튼에 연결)
    /// </summary>
    public void OnNextWeaponButtonClicked()
    {
        if (weaponManager != null)
        {
            weaponManager.SwitchToNextWeapon();
        }
    }

    /// <summary>
    /// 이전 무기로 전환 버튼 이벤트 (UI 버튼에 연결)
    /// </summary>
    public void OnPreviousWeaponButtonClicked()
    {
        if (weaponManager != null)
        {
            weaponManager.SwitchToPreviousWeapon();
        }
    }
}