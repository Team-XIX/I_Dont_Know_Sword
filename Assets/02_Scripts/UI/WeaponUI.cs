using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private WeaponManager weaponManager;

    [Header("UI 요소")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponIndexText; // "1/3" 같은 형식

    [Header("무기 아이콘")]
    [SerializeField] private List<WeaponIconData> weaponIcons = new List<WeaponIconData>();
    [SerializeField] private Sprite defaultWeaponIcon; // 아이콘이 없을 때 표시할 기본 이미지

    private void Start()
    {
        // WeaponManager 찾기
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }

        if (weaponManager != null)
        {
            // 무기 변경 이벤트 구독
            weaponManager.OnWeaponChanged += OnWeaponChanged;

            // 초기 무기 UI 업데이트
            WeaponData currentWeapon = weaponManager.GetCurrentWeaponData();
            if (currentWeapon != null)
            {
                UpdateWeaponUI(weaponManager.GetCurrentWeaponIndex(), weaponManager.GetWeaponCount(), currentWeapon);
            }
        }
        else
        {
            Debug.LogWarning("WeaponManager를 찾을 수 없습니다.");
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

        // 무기 아이콘 업데이트
        if (weaponIcon != null && weaponData != null)
        {
            Sprite icon = GetWeaponIconById(weaponData.id);
            weaponIcon.sprite = icon != null ? icon : defaultWeaponIcon;
        }
    }

    /// <summary>
    /// 무기 ID에 해당하는 아이콘 가져오기
    /// </summary>
    private Sprite GetWeaponIconById(int weaponId)
    {
        foreach (WeaponIconData iconData in weaponIcons)
        {
            if (iconData.weaponId == weaponId)
            {
                return iconData.icon;
            }
        }
        return defaultWeaponIcon;
    }
}

/// <summary>
/// 무기 아이콘 데이터
/// </summary>
[System.Serializable]
public class WeaponIconData
{
    public int weaponId;
    public Sprite icon;
}