using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

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
    /// 새 무기 장착 (데이터만)
    /// </summary>
    public void EquipWeapon(WeaponData weaponData)
    {
        EquipWeapon(weaponData, null);
    }

    /// <summary>
    /// 새 무기 장착 (데이터와 스프라이트)
    /// </summary>
    public void EquipWeapon(WeaponData weaponData, Sprite weaponSprite)
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

            // 무기 스프라이트 업데이트
            UpdateWeaponVisuals(weaponSprite);
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
                weaponSpriteRenderer.sprite = null;
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
    private void UpdateWeaponVisuals(Sprite weaponSprite = null)
    {
        if (weaponSpriteRenderer != null)
        {
            if (weaponSprite != null)
            {
                // 전달받은 스프라이트 사용
                weaponSpriteRenderer.sprite = weaponSprite;
                Debug.Log($"Weapon sprite updated: {weaponSprite.name}");
            }
            else if (currentWeaponData != null)
            {
                // 스프라이트가 없다면 아틀라스에서 로드 시도
                SpriteAtlas atlas = Resources.Load<SpriteAtlas>("WeaponSprite/Weapon");
                if (atlas != null)
                {
                    Sprite sprite = atlas.GetSprite($"{currentWeaponData.id}");
                    if (sprite != null)
                    {
                        weaponSpriteRenderer.sprite = sprite;
                        Debug.Log($"Weapon sprite loaded from atlas: {sprite.name}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 현재 장착된 무기 데이터 반환
    /// </summary>
    public WeaponData GetCurrentWeaponData()
    {
        return currentWeaponData;
    }
}