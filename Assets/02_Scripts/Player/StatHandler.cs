using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    // SingleTon을 쓰지 않은 이유는 DontDestroyOnLoad를 사용하지 않기 때문
    #region 싱글톤 구현
    public static StatHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentHealth = MaxHealth;
    }
    #endregion

    // 어떤 스탯이 변경되더라도 이 이벤트가 호출해서 UI 업데이트 해야함!
    public event Action OnStatsChanged; // 실제 기능이 없음. UpdateUI()

    #region 기본 스탯 속성
    [Header("기본 스탯 설정")]
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _attackPower = 10f;
    [SerializeField] private float _moveSpeed = 5f;

    // 현재 체력
    [SerializeField] private int _currentHealth;

    // 스탯 속성 (get/set)
    public int MaxHealth
    {
        get => _maxHealth;
        set
        {
            int oldMaxHealth = _maxHealth;
            _maxHealth = Mathf.Max(1, value); // 최대 체력은 최소 1 이상

            // 현재 체력 비율 유지 (예: 최대 체력의 50%였다면 변경 후에도 50% 유지)
            if (oldMaxHealth > 0)
            {
                float healthRatio = (float)_currentHealth / oldMaxHealth;
                _currentHealth = Mathf.RoundToInt(healthRatio * _maxHealth);
            }
            else
            {
                _currentHealth = _maxHealth;
            }

            OnStatsChanged?.Invoke();
        }
    }

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
            // 이벤트 발생
            OnStatsChanged?.Invoke();
        }
    }

    public float AttackPower
    {
        get => _attackPower;
        set
        {
            _attackPower = value;
            // 이벤트 발생
            OnStatsChanged?.Invoke();
        }
    }

    public float MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            _moveSpeed = value;
            // 이벤트 발생
            OnStatsChanged?.Invoke();
        }
    }
    #endregion

    #region 투사체 스탯 속성
    [Header("투사체 스탯 설정")]
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _spreadAngle = 0f;  // 탄퍼짐 (랜덤 각도의 최대값)
    [SerializeField] private float _multiAngle = 0f;  // 확산각 (여러 발 발사 시 퍼지는 각도)
    [SerializeField] private float _projectileSize = 1f;
    [SerializeField] private float _projectileSpeed = 0f;
    [SerializeField] private float _projectileRange = 0f;  // 투사체가 남는 시간(초)
    [SerializeField] private int _reflectionCount = 0;  // 반사 횟수
    [SerializeField] private int _penetrationCount = 0;  // 관통 횟수

    // 투사체 속성 (get/set)
    public float AttackSpeed
    {
        get => _attackSpeed;
        set
        {
            _attackSpeed = Mathf.Max(0.1f, value);  // 최소 공격 속도 제한
            OnStatsChanged?.Invoke();
        }
    }

    public float SpreadAngle
    {
        get => _spreadAngle;
        set
        {
            _spreadAngle = Mathf.Max(0f, value);
            OnStatsChanged?.Invoke();
        }
    }

    public float MultiAngle
    {
        get => _multiAngle;
        set
        {
            _multiAngle = Mathf.Max(0f, value);
            OnStatsChanged?.Invoke();
        }
    }

    public float ProjectileSize
    {
        get => _projectileSize;
        set
        {
            _projectileSize = Mathf.Max(0.1f, value);  // 최소 크기 제한
            OnStatsChanged?.Invoke();
        }
    }

    public float ProjectileSpeed
    {
        get => _projectileSpeed;
        set
        {
            _projectileSpeed = Mathf.Max(0.1f, value);  // 최소 속도 제한
            OnStatsChanged?.Invoke();
        }
    }

    public float ProjectileRange
    {
        get => _projectileRange;
        set
        {
            _projectileRange = Mathf.Max(0.1f, value);  // 최소 시간 제한
            OnStatsChanged?.Invoke();
        }
    }

    public int ReflectionCount
    {
        get => _reflectionCount;
        set
        {
            _reflectionCount = value;
            OnStatsChanged?.Invoke();
        }
    }

    public int PenetrationCount
    {
        get => _penetrationCount;
        set
        {
            _penetrationCount = value;
            OnStatsChanged?.Invoke();
        }
    }
    #endregion
}