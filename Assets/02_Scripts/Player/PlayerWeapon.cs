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
    /// 무기 장착
    /// </summary>
    public void EquipWeapon(WeaponData weaponData)
    {

        if (isWeaponEquipped && currentWeaponData != null)
        {
            RemoveWeaponStats();
        }

        currentWeaponData = weaponData;

        if (currentWeaponData != null)
        {
            ApplyWeaponStats();
            isWeaponEquipped = true;

            // 무기 스프라이트 업데이트 (아틀라스에서 직접 로드)
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
                // 스프라이트 설정
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
        if (weaponSpriteRenderer != null && currentWeaponData != null)
        {
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>("WeaponSprite/Weapon");
            if (atlas != null)
            {
                Sprite sprite = atlas.GetSprite($"{currentWeaponData.id}");
                if (sprite != null)
                {
                    weaponSpriteRenderer.sprite = sprite;
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