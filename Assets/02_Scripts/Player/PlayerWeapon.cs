using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("무기 참조")]
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    private WeaponData currentWeaponData;
    private bool isWeaponEquipped = false;

    private void Awake()
    {
        if (weaponSpriteRenderer == null)
        {
            weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// 새 무기 장착
    /// </summary>
    public void EquipWeapon(WeaponData weaponData)
    {
        Debug.Log($"EquipWeapon: 무기 데이터={weaponData?.name}, StatHandler={StatHandler.Instance != null}");

        if (isWeaponEquipped && currentWeaponData != null)
        {
            RemoveWeaponStats();
        }

        currentWeaponData = weaponData;

        if (currentWeaponData != null)
        {
            ApplyWeaponStats();
            isWeaponEquipped = true;

            // 무기 스프라이트 업데이트 (스프라이트가 있다면)
            UpdateWeaponVisuals();
        }
    }

    /// <summary>
    /// 무기 해제
    /// </summary>
    public void UnequipWeapon()
    {
        if (isWeaponEquipped && currentWeaponData != null)
        {
            RemoveWeaponStats();
            currentWeaponData = null;
            isWeaponEquipped = false;

            if (weaponSpriteRenderer != null)
            {
                // 기본 스프라이트 설정
            }
        }
    }

    /// <summary>
    /// 비활성화될 때 무기 스탯 제거
    /// </summary>
    private void OnDisable()
    {
        if (isWeaponEquipped && currentWeaponData != null)
        {
            RemoveWeaponStats();
        }
    }

    /// <summary>
    /// 활성화될 때 무기 스탯 적용
    /// </summary>
    private void OnEnable()
    {
        if (isWeaponEquipped && currentWeaponData != null)
        {
            ApplyWeaponStats();
        }
    }

    /// <summary>
    /// 무기의 능력치를 StatHandler에 적용
    /// </summary>
    private void ApplyWeaponStats()
    {
        StatHandler statHandler = StatHandler.Instance;
        if (statHandler == null || currentWeaponData == null) return;

        // 여러 스탯을 한번에 변경하여 이벤트 발생 최소화
        statHandler.ModifyMultipleStats(() =>
        {
            statHandler.AdditionalAttackPower += currentWeaponData.atk;
            statHandler.AdditionalMoveSpeed += currentWeaponData.moveSpeed;
            statHandler.AdditionalAttackSpeed += currentWeaponData.atkSpeed;
            statHandler.AdditionalSpreadAngle += currentWeaponData.spreadAngle;
            statHandler.AdditionalMultiAngle += currentWeaponData.multiAngle;
            statHandler.AdditionalProjectileCount += currentWeaponData.projectileCnt;
            statHandler.AdditionalProjectileSize += currentWeaponData.projectileSize;
            statHandler.AdditionalProjectileSpeed += currentWeaponData.projectileSpeed;
            statHandler.AdditionalProjectileRange += currentWeaponData.projectileRange;
            statHandler.AdditionalReflectionCount += currentWeaponData.reflectionCnt;
            statHandler.AdditionalPenetrationCount += currentWeaponData.penetrationCnt;

            if (currentWeaponData.autoFire)
            {
                statHandler.AdditionalAutoFire = true;
            }
        });

        // 로그 출력 (디버깅용)
        Debug.Log($"무기 [{currentWeaponData.name}] 능력치 적용됨");
    }

    /// <summary>
    /// 무기의 능력치를 StatHandler에서 제거
    /// </summary>
    private void RemoveWeaponStats()
    {
        StatHandler statHandler = StatHandler.Instance;
        if (statHandler == null || currentWeaponData == null) return;

        statHandler.ModifyMultipleStats(() =>
        {
            statHandler.AdditionalAttackPower -= currentWeaponData.atk;
            statHandler.AdditionalMoveSpeed -= currentWeaponData.moveSpeed;
            statHandler.AdditionalAttackSpeed -= currentWeaponData.atkSpeed;
            statHandler.AdditionalSpreadAngle -= currentWeaponData.spreadAngle;
            statHandler.AdditionalMultiAngle -= currentWeaponData.multiAngle;
            statHandler.AdditionalProjectileCount -= currentWeaponData.projectileCnt;
            statHandler.AdditionalProjectileSize -= currentWeaponData.projectileSize;
            statHandler.AdditionalProjectileSpeed -= currentWeaponData.projectileSpeed;
            statHandler.AdditionalProjectileRange -= currentWeaponData.projectileRange;
            statHandler.AdditionalReflectionCount -= currentWeaponData.reflectionCnt;
            statHandler.AdditionalPenetrationCount -= currentWeaponData.penetrationCnt;

            if (currentWeaponData.autoFire)
            {
                statHandler.AdditionalAutoFire = false;
            }
        });
    }

    /// <summary>
    /// 무기 시각적 요소(스프라이트) 업데이트
    /// </summary>
    private void UpdateWeaponVisuals()
    {
        // 무기 스프라이트를 로드하는 코드 추가(아마도)
        // Sprite weaponSprite = ResourceManager.Instance.GetWeaponSprite(currentWeaponData.id);
        // if (weaponSprite != null && weaponSpriteRenderer != null)
        // {
        //     weaponSpriteRenderer.sprite = weaponSprite;
        // }
    }

    /// <summary>
    /// 현재 장착된 무기 데이터 반환
    /// </summary>
    public WeaponData GetCurrentWeaponData()
    {
        return currentWeaponData;
    }
}