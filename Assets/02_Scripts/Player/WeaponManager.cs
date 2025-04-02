using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("무기 설정")]
    [SerializeField] private List<WeaponData> weaponDataList = new List<WeaponData>();
    [SerializeField] private int currentWeaponIndex = 0;
    [SerializeField] private PlayerWeapon playerWeaponComponent;
    [SerializeField] private int startingWeaponId = 1; // 시작 무기 ID

    public delegate void WeaponChangedHandler(int index, int total, WeaponData currentWeapon);
    public event WeaponChangedHandler OnWeaponChanged;

    private void Start()
    {
        if (playerWeaponComponent == null)
        {
            playerWeaponComponent = GetComponentInChildren<PlayerWeapon>();
        }

        StartCoroutine(InitializeStartingWeapon());
    }

    /// <summary>
    /// DataManager가 데이터를 로드할 때까지 기다린 후 시작 무기 설정
    /// </summary>
    private IEnumerator InitializeStartingWeapon()
    {
        while (DataManager.Instance == null || DataManager.Instance.weaponCount <= 1)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 시작 무기가 이미 리스트에 있는지 확인
        bool hasStartingWeapon = false;
        foreach (WeaponData weapon in weaponDataList)
        {
            if (weapon != null && weapon.id == startingWeaponId)
            {
                hasStartingWeapon = true;
                break;
            }
        }

        // 시작 무기가 없으면 추가
        if (!hasStartingWeapon)
        {
            WeaponData startingWeapon = DataManager.Instance.GetWeaponById(startingWeaponId);
            if (startingWeapon != null)
            {
                AddWeapon(startingWeapon);
                //AddWeapon(DataManager.Instance.GetWeaponById(2));
                //AddWeapon(DataManager.Instance.GetWeaponById(3));
                //AddWeapon(DataManager.Instance.GetWeaponById(4));
            }
        }
        else if (weaponDataList.Count > 0)
        {
            // 이미 무기가 있다면 첫 번째 무기 장착
            UpdateActiveWeapon();
        }
    }

    /// <summary>
    /// 현재 무기 인덱스를 반환
    /// </summary>
    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }

    /// <summary>
    /// 현재 활성화된 무기 데이터 반환
    /// </summary>
    public WeaponData GetCurrentWeaponData()
    {
        if (weaponDataList.Count == 0 || currentWeaponIndex < 0 || currentWeaponIndex >= weaponDataList.Count)
            return null;

        return weaponDataList[currentWeaponIndex];
    }

    /// <summary>
    /// 총 무기 개수 반환
    /// </summary>
    public int GetWeaponCount()
    {
        return weaponDataList.Count;
    }

    /// <summary>
    /// 이전 무기로 전환
    /// </summary>
    public void SwitchToPreviousWeapon()
    {
        if (weaponDataList.Count <= 1) return;

        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weaponDataList.Count - 1;
        }

        UpdateActiveWeapon();
    }

    /// <summary>
    /// 다음 무기로 전환
    /// </summary>
    public void SwitchToNextWeapon()
    {
        if (weaponDataList.Count <= 1) return;

        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponDataList.Count)
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
        if (weaponDataList.Count == 0) return;

        // 유효한 인덱스 범위로 제한
        index = Mathf.Clamp(index, 0, weaponDataList.Count - 1);

        if (currentWeaponIndex == index) return;

        currentWeaponIndex = index;
        UpdateActiveWeapon();
    }

    /// <summary>
    /// 무기 객체로부터 데이터를 추출하여 무기 추가
    /// </summary>
    public void AddWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            return;
        }

        WeaponData weaponData = weapon.weaponData;
        if (weaponData == null)
        {
            return;
        }

        AddWeapon(weaponData);
    }

    /// <summary>
    /// 무기 데이터로 무기 추가
    /// </summary>
    public void AddWeapon(WeaponData weaponData)
    {
        if (weaponData != null && !weaponDataList.Contains(weaponData))
        {
            weaponDataList.Add(weaponData);

            // 처음 추가된 무기라면 자동으로 장착
            if (weaponDataList.Count == 1)
            {
                currentWeaponIndex = 0;
                UpdateActiveWeapon();
            }
            else if (OnWeaponChanged != null)
            {
                // 현재 선택된 무기 데이터
                WeaponData currentWeapon = GetCurrentWeaponData();
                OnWeaponChanged.Invoke(currentWeaponIndex, weaponDataList.Count, currentWeapon);
            }
        }
    }


    /// <summary>
    /// 활성화된 무기 업데이트
    /// </summary>
    private void UpdateActiveWeapon()
    {
        WeaponData currentWeapon = GetCurrentWeaponData();

        if (currentWeapon != null && playerWeaponComponent != null)
        {
            // PlayerWeapon에 현재 무기 데이터 설정 (스프라이트는 PlayerWeapon 내부에서 로드)
            playerWeaponComponent.EquipWeapon(currentWeapon);

            // 무기 변경 이벤트 발생 (UI 업데이트용)
            if (OnWeaponChanged != null)
            {
                OnWeaponChanged.Invoke(currentWeaponIndex, weaponDataList.Count, currentWeapon);
            }
        }
    }
}