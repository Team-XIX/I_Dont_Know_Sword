using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("무기 기본 정보")]
    [SerializeField] private string weaponName = "기본 무기";
    [SerializeField] private int weaponId = 0;
    [SerializeField] private Sprite weaponSprite;

    [Header("추가 능력치")]
    [SerializeField] private int additionalMaxHealth = 0;
    [SerializeField] private float additionalAttackPower = 0f;
    [SerializeField] private float additionalMoveSpeed = 0f;
    [SerializeField] private float additionalAttackSpeed = 0f;
    [SerializeField] private bool additionalAutoFire = false;
    [SerializeField] private float additionalSpreadAngle = 0f;
    [SerializeField] private float additionalMultiAngle = 0f;
    [SerializeField] private int additionalProjectileCount = 0;
    [SerializeField] private float additionalProjectileSize = 0f;
    [SerializeField] private float additionalProjectileSpeed = 0f;
    [SerializeField] private float additionalProjectileRange = 0f;
    [SerializeField] private int additionalReflectionCount = 0;
    [SerializeField] private int additionalPenetrationCount = 0;

    private SpriteRenderer spriteRenderer;
    private bool isEquipped = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer && weaponSprite)
        {
            spriteRenderer.sprite = weaponSprite;
        }
    }

    private void OnEnable()
    {
        // 무기가 활성화될 때 StatHandler에 능력치 적용
        ApplyWeaponStats(true);
        isEquipped = true;
    }

    private void OnDisable()
    {
        // 무기가 비활성화될 때 StatHandler에서 능력치 제거
        if (isEquipped)
        {
            ApplyWeaponStats(false);
            isEquipped = false;
        }
    }

    /// <summary>
    /// 무기의 능력치를 StatHandler에 적용 또는 제거
    /// </summary>
    /// <param name="apply">true: 적용, false: 제거</param>
    private void ApplyWeaponStats(bool apply)
    {
        StatHandler statHandler = StatHandler.Instance;
        if (statHandler == null) return;

        // 여러 스탯을 한번에 변경하여 이벤트 발생 최소화
        statHandler.ModifyMultipleStats(() =>
        {
            // 추가 능력치를 적용하거나 제거
            int multiplier = apply ? 1 : -1;

            statHandler.AdditionalMaxHealth += additionalMaxHealth * multiplier;
            statHandler.AdditionalAttackPower += additionalAttackPower * multiplier;
            statHandler.AdditionalMoveSpeed += additionalMoveSpeed * multiplier;
            statHandler.AdditionalAttackSpeed += additionalAttackSpeed * multiplier;

            // 불리언 값은 특별하게 처리
            if (additionalAutoFire)
            {
                statHandler.AdditionalAutoFire = apply ? true : false;
            }

            statHandler.AdditionalSpreadAngle += additionalSpreadAngle * multiplier;
            statHandler.AdditionalMultiAngle += additionalMultiAngle * multiplier;
            statHandler.AdditionalProjectileCount += additionalProjectileCount * multiplier;
            statHandler.AdditionalProjectileSize += additionalProjectileSize * multiplier;
            statHandler.AdditionalProjectileSpeed += additionalProjectileSpeed * multiplier;
            statHandler.AdditionalProjectileRange += additionalProjectileRange * multiplier;
            statHandler.AdditionalReflectionCount += additionalReflectionCount * multiplier;
            statHandler.AdditionalPenetrationCount += additionalPenetrationCount * multiplier;
        });

        // 로그 출력 (디버깅용)
        if (apply)
        {
            Debug.Log($"무기 [{weaponName}] 능력치 적용됨");
        }
        else
        {
            Debug.Log($"무기 [{weaponName}] 능력치 제거됨");
        }
    }

    /// <summary>
    /// 무기 ID 반환
    /// </summary>
    public int GetWeaponId()
    {
        return weaponId;
    }

    /// <summary>
    /// 무기 이름 반환
    /// </summary>
    public string GetWeaponName()
    {
        return weaponName;
    }

    /// <summary>
    /// 무기 스프라이트 반환
    /// </summary>
    public Sprite GetWeaponSprite()
    {
        return weaponSprite;
    }
}