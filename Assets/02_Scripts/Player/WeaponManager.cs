using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("무기 설정")]
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();
    [SerializeField] private int currentWeaponIndex = 0;

    // UI 연결을 위한 이벤트
    public delegate void WeaponChangedHandler(int index, int total, PlayerWeapon currentWeapon);
    public event WeaponChangedHandler OnWeaponChanged;

    // 이전의 무기 변경 이벤트(호환성 유지)
    public delegate void WeaponChangedLegacyHandler(int index, int total);
    public event WeaponChangedLegacyHandler OnWeaponChangedLegacy;

    private void Start()
    {
        // 시작할 때 올바른 무기만 활성화
        UpdateActiveWeapon();
    }

    /// <summary>
    /// 현재 무기 인덱스를 반환
    /// </summary>
    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }

    /// <summary>
    /// 현재 활성화된 무기 GameObject 반환
    /// </summary>
    public GameObject GetCurrentWeapon()
    {
        if (weapons.Count == 0) return null;
        return weapons[currentWeaponIndex];
    }

    /// <summary>
    /// 현재 활성화된 무기의 PlayerWeapon 컴포넌트 반환
    /// </summary>
    public PlayerWeapon GetCurrentWeaponComponent()
    {
        GameObject currentWeapon = GetCurrentWeapon();
        if (currentWeapon == null) return null;

        return currentWeapon.GetComponent<PlayerWeapon>();
    }

    /// <summary>
    /// 이전 무기로 전환
    /// </summary>
    public void SwitchToPreviousWeapon()
    {
        if (weapons.Count <= 1) return;

        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weapons.Count - 1;
        }

        UpdateActiveWeapon();
    }

    /// <summary>
    /// 다음 무기로 전환
    /// </summary>
    public void SwitchToNextWeapon()
    {
        if (weapons.Count <= 1) return;

        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Count)
        {
            currentWeaponIndex = 0;
        }

        UpdateActiveWeapon();
    }

    /// <summary>
    /// 특정 인덱스의 무기로 전환
    /// </summary>
    public void SwitchToWeapon(int index)
    {
        if (weapons.Count == 0) return;

        // 유효한 인덱스 범위로 제한
        index = Mathf.Clamp(index, 0, weapons.Count - 1);

        if (currentWeaponIndex == index) return;

        currentWeaponIndex = index;
        UpdateActiveWeapon();
    }

    /// <summary>
    /// 무기 목록에 새 무기 추가
    /// </summary>
    public void AddWeapon(GameObject weapon)
    {
        if (weapon != null && !weapons.Contains(weapon))
        {
            // 무기에 PlayerWeapon 컴포넌트가 있는지 확인
            if (weapon.GetComponent<PlayerWeapon>() == null)
            {
                Debug.LogWarning("PlayerWeapon 못찾음");
            }

            weapons.Add(weapon);
            weapon.SetActive(false); // 처음에는 비활성화
            UpdateActiveWeapon();
        }
    }

    /// <summary>
    /// 활성화된 무기 업데이트
    /// </summary>
    private void UpdateActiveWeapon()
    {
        // 모든 무기 비활성화 (OnDisable이 호출되어 자동으로 스탯 제거)
        foreach (GameObject weapon in weapons)
        {
            if (weapon != null)
            {
                weapon.SetActive(false);
            }
        }

        // 현재 선택된 무기만 활성화 (OnEnable이 호출되어 자동으로 스탯 적용)
        if (weapons.Count > 0 && currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Count)
        {
            GameObject currentWeapon = weapons[currentWeaponIndex];
            if (currentWeapon != null)
            {
                currentWeapon.SetActive(true);

                // 무기 컴포넌트 가져오기
                PlayerWeapon weaponComponent = currentWeapon.GetComponent<PlayerWeapon>();

                // 무기 변경 이벤트 발생 (UI 업데이트용)
                OnWeaponChangedLegacy?.Invoke(currentWeaponIndex, weapons.Count);
                OnWeaponChanged?.Invoke(currentWeaponIndex, weapons.Count, weaponComponent);

                // 디버그 로그
                string weaponName = weaponComponent != null ? weaponComponent.GetWeaponName() : "Unknown";
                Debug.Log($"무기 전환: {weaponName} (인덱스: {currentWeaponIndex})");
            }
        }
    }
}