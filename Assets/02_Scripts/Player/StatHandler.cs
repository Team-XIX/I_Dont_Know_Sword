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
        UpdateCurrentHealthToMax();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    // 이벤트
    public event Action OnHealthChanged;
    public event Action OnPlayerDeath;

    private bool _suppressEvents = false;

    #region 기본 스탯 속성
    [Header("기본 스탯 설정")]
    [SerializeField] private int _baseMaxHealth = 3;
    [SerializeField] private float _baseAttackPower = 10f;
    [SerializeField] private float _baseMoveSpeed = 5f;

    // 현재 체력
    [SerializeField] private int _currentHealth;

    // 추가 스탯 (아이템, 버프 등에 의한 추가 능력치) - 인스펙터에 표시
    [Header("추가 스탯 (읽기 전용)")]
    [SerializeField] private int _additionalMaxHealth = 0;
    [SerializeField] private float _additionalAttackPower = 0f;
    [SerializeField] private float _additionalMoveSpeed = 0f;

    // 계산된 최종 능력치 속성 - 인스펙터에 표시
    [Header("최종 능력치 (읽기 전용)")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _attackPower;
    [SerializeField] private float _moveSpeed;

    // 최종 능력치 프로퍼티 (게임 코드에서 사용)
    public int MaxHealth
    {
        get => _maxHealth;
        private set => _maxHealth = value;
    }

    public float AttackPower
    {
        get => _attackPower;
        private set => _attackPower = value;
    }

    public float MoveSpeed
    {
        get => _moveSpeed;
        private set => _moveSpeed = value;
    }

    // 기본 스탯 속성 (get/set)
    public int BaseMaxHealth
    {
        get => _baseMaxHealth;
        set
        {
            _baseMaxHealth = Mathf.Max(1, value); // 최대 체력은 최소 1 이상
            RecalculateMaxHealth();
        }
    }

    public float BaseAttackPower
    {
        get => _baseAttackPower;
        set
        {
            _baseAttackPower = value;
            RecalculateAttackPower();
        }
    }

    public float BaseMoveSpeed
    {
        get => _baseMoveSpeed;
        set
        {
            _baseMoveSpeed = value;
            RecalculateMoveSpeed();
        }
    }

    // 추가 스탯 속성 (get/set)
    public int AdditionalMaxHealth
    {
        get => _additionalMaxHealth;
        set
        {
            _additionalMaxHealth = value;
            RecalculateMaxHealth();
        }
    }

    public float AdditionalAttackPower
    {
        get => _additionalAttackPower;
        set
        {
            _additionalAttackPower = value;
            RecalculateAttackPower();
        }
    }

    public float AdditionalMoveSpeed
    {
        get => _additionalMoveSpeed;
        set
        {
            _additionalMoveSpeed = value;
            RecalculateMoveSpeed();
        }
    }

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            int oldHealth = _currentHealth;
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);

            // 이벤트 발생 (이벤트 억제가 아니고, 체력이 변경된 경우에만)
            if (!_suppressEvents && oldHealth != _currentHealth)
            {
                OnHealthChanged?.Invoke();

                // 체력이 0이 되면 죽음 이벤트 발생
                if (_currentHealth <= 0)
                {
                    OnPlayerDeath?.Invoke();
                }
            }
        }
    }

    // 최종 능력치 재계산 메서드
    private void RecalculateMaxHealth()
    {
        int oldMaxHealth = MaxHealth;
        MaxHealth = _baseMaxHealth + _additionalMaxHealth;

        // 최대 체력이 증가한 경우, 증가한 만큼 현재 체력도 증가
        if (MaxHealth > oldMaxHealth && oldMaxHealth > 0)
        {
            int healthIncrease = MaxHealth - oldMaxHealth;
            int oldHealth = _currentHealth;
            _currentHealth += healthIncrease;

            // 체력이 변경되었을 때만 이벤트 발생
            if (!_suppressEvents && oldHealth != _currentHealth)
            {
                OnHealthChanged?.Invoke();
            }
        }
        // 최대 체력이 감소한 경우, 현재 체력이 최대 체력을 초과하지 않도록 조정
        else if (MaxHealth < oldMaxHealth)
        {
            int oldHealth = _currentHealth;
            _currentHealth = Mathf.Min(_currentHealth, MaxHealth);

            // 체력이 변경되었을 때만 이벤트 발생
            if (!_suppressEvents && oldHealth != _currentHealth)
            {
                OnHealthChanged?.Invoke();
            }
        }
    }

    private void RecalculateAttackPower()
    {
        AttackPower = _baseAttackPower + _additionalAttackPower;
    }

    private void RecalculateMoveSpeed()
    {
        MoveSpeed = _baseMoveSpeed + _additionalMoveSpeed;
    }

    private void UpdateCurrentHealthToMax()
    {
        MaxHealth = _baseMaxHealth + _additionalMaxHealth;
        _currentHealth = MaxHealth;
    }
    #endregion

    #region 투사체 스탯 속성
    [Header("투사체 기본 스탯")]
    [SerializeField] private float _baseAttackSpeed = 1f;
    [SerializeField] private bool _baseAutoFire = false;  // 연사 가능 여부
    [SerializeField] private float _baseSpreadAngle = 0f;  // 탄퍼짐 (랜덤 각도의 최대값)
    [SerializeField] private float _baseMultiAngle = 0f;  // 확산각 (여러 발 발사 시 퍼지는 각도)
    [SerializeField] private int _baseProjectileCount = 1;  // 한 번에 발사되는 투사체 개수
    [SerializeField] private float _baseProjectileSize = 1f;
    [SerializeField] private float _baseProjectileSpeed = 10f;
    [SerializeField] private float _baseProjectileRange = 5f;  // 투사체가 남는 시간(초)
    [SerializeField] private int _baseReflectionCount = 0;  // 반사 횟수
    [SerializeField] private int _basePenetrationCount = 0;  // 관통 횟수

    // 투사체 추가 스탯 - 인스펙터에 표시
    [Header("추가 투사체 스탯 (읽기 전용)")]
    [SerializeField] private float _additionalAttackSpeed = 0f;
    [SerializeField] private bool _additionalAutoFire = false;
    [SerializeField] private float _additionalSpreadAngle = 0f;
    [SerializeField] private float _additionalMultiAngle = 0f;
    [SerializeField] private int _additionalProjectileCount = 0;
    [SerializeField] private float _additionalProjectileSize = 0f;
    [SerializeField] private float _additionalProjectileSpeed = 0f;
    [SerializeField] private float _additionalProjectileRange = 0f;
    [SerializeField] private int _additionalReflectionCount = 0;
    [SerializeField] private int _additionalPenetrationCount = 0;

    // 최종 투사체 스탯 - 인스펙터에 표시
    [Header("최종 투사체 스탯 (읽기 전용)")]
    [SerializeField] private float _attackSpeed;
    [SerializeField] private bool _autoFire;
    [SerializeField] private float _spreadAngle;
    [SerializeField] private float _multiAngle;
    [SerializeField] private int _projectileCount;
    [SerializeField] private float _projectileSize;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileRange;
    [SerializeField] private int _reflectionCount;
    [SerializeField] private int _penetrationCount;

    // 최종 투사체 스탯 프로퍼티 (게임 코드에서 사용)
    public float AttackSpeed
    {
        get => _attackSpeed;
        private set => _attackSpeed = value;
    }

    public bool AutoFire
    {
        get => _autoFire;
        private set => _autoFire = value;
    }

    public float SpreadAngle
    {
        get => _spreadAngle;
        private set => _spreadAngle = value;
    }

    public float MultiAngle
    {
        get => _multiAngle;
        private set => _multiAngle = value;
    }

    public int ProjectileCount
    {
        get => _projectileCount;
        private set => _projectileCount = value;
    }

    public float ProjectileSize
    {
        get => _projectileSize;
        private set => _projectileSize = value;
    }

    public float ProjectileSpeed
    {
        get => _projectileSpeed;
        private set => _projectileSpeed = value;
    }

    public float ProjectileRange
    {
        get => _projectileRange;
        private set => _projectileRange = value;
    }

    public int ReflectionCount
    {
        get => _reflectionCount;
        private set => _reflectionCount = value;
    }

    public int PenetrationCount
    {
        get => _penetrationCount;
        private set => _penetrationCount = value;
    }

    // 기본 투사체 스탯 속성 (get/set)
    public float BaseAttackSpeed
    {
        get => _baseAttackSpeed;
        set
        {
            _baseAttackSpeed = Mathf.Max(0.1f, value);
            RecalculateAttackSpeed();
        }
    }

    public bool BaseAutoFire
    {
        get => _baseAutoFire;
        set
        {
            _baseAutoFire = value;
            RecalculateAutoFire();
        }
    }

    public float BaseSpreadAngle
    {
        get => _baseSpreadAngle;
        set
        {
            _baseSpreadAngle = Mathf.Max(0f, value);
            RecalculateSpreadAngle();
        }
    }

    public float BaseMultiAngle
    {
        get => _baseMultiAngle;
        set
        {
            _baseMultiAngle = Mathf.Max(0f, value);
            RecalculateMultiAngle();
        }
    }

    public int BaseProjectileCount
    {
        get => _baseProjectileCount;
        set
        {
            _baseProjectileCount = Mathf.Max(1, value);  // 최소 1개 이상
            RecalculateProjectileCount();
        }
    }

    public float BaseProjectileSize
    {
        get => _baseProjectileSize;
        set
        {
            _baseProjectileSize = Mathf.Max(0.1f, value);
            RecalculateProjectileSize();
        }
    }

    public float BaseProjectileSpeed
    {
        get => _baseProjectileSpeed;
        set
        {
            _baseProjectileSpeed = Mathf.Max(0.1f, value);
            RecalculateProjectileSpeed();
        }
    }

    public float BaseProjectileRange
    {
        get => _baseProjectileRange;
        set
        {
            _baseProjectileRange = Mathf.Max(0.1f, value);
            RecalculateProjectileRange();
        }
    }

    public int BaseReflectionCount
    {
        get => _baseReflectionCount;
        set
        {
            _baseReflectionCount = value;
            RecalculateReflectionCount();
        }
    }

    public int BasePenetrationCount
    {
        get => _basePenetrationCount;
        set
        {
            _basePenetrationCount = value;
            RecalculatePenetrationCount();
        }
    }

    // 추가 투사체 스탯 속성
    public float AdditionalAttackSpeed
    {
        get => _additionalAttackSpeed;
        set
        {
            _additionalAttackSpeed = value;
            RecalculateAttackSpeed();
        }
    }

    public bool AdditionalAutoFire
    {
        get => _additionalAutoFire;
        set
        {
            _additionalAutoFire = value;
            RecalculateAutoFire();
        }
    }

    public float AdditionalSpreadAngle
    {
        get => _additionalSpreadAngle;
        set
        {
            _additionalSpreadAngle = value;
            RecalculateSpreadAngle();
        }
    }

    public float AdditionalMultiAngle
    {
        get => _additionalMultiAngle;
        set
        {
            _additionalMultiAngle = value;
            RecalculateMultiAngle();
        }
    }

    public int AdditionalProjectileCount
    {
        get => _additionalProjectileCount;
        set
        {
            _additionalProjectileCount = value;
            RecalculateProjectileCount();
        }
    }

    public float AdditionalProjectileSize
    {
        get => _additionalProjectileSize;
        set
        {
            _additionalProjectileSize = value;
            RecalculateProjectileSize();
        }
    }

    public float AdditionalProjectileSpeed
    {
        get => _additionalProjectileSpeed;
        set
        {
            _additionalProjectileSpeed = value;
            RecalculateProjectileSpeed();
        }
    }

    public float AdditionalProjectileRange
    {
        get => _additionalProjectileRange;
        set
        {
            _additionalProjectileRange = value;
            RecalculateProjectileRange();
        }
    }

    public int AdditionalReflectionCount
    {
        get => _additionalReflectionCount;
        set
        {
            _additionalReflectionCount = value;
            RecalculateReflectionCount();
        }
    }

    public int AdditionalPenetrationCount
    {
        get => _additionalPenetrationCount;
        set
        {
            _additionalPenetrationCount = value;
            RecalculatePenetrationCount();
        }
    }

    // 투사체 스탯 재계산 메서드
    private void RecalculateAttackSpeed()
    {
        AttackSpeed = Mathf.Max(0.1f, _baseAttackSpeed + _additionalAttackSpeed);
    }

    private void RecalculateAutoFire()
    {
        // 기본값이나 추가값 중 하나라도 true이면 연사 가능
        AutoFire = _baseAutoFire || _additionalAutoFire;
    }

    private void RecalculateSpreadAngle()
    {
        SpreadAngle = Mathf.Max(0f, _baseSpreadAngle + _additionalSpreadAngle);
    }

    private void RecalculateMultiAngle()
    {
        MultiAngle = Mathf.Max(0f, _baseMultiAngle + _additionalMultiAngle);
    }

    private void RecalculateProjectileCount()
    {
        ProjectileCount = Mathf.Max(1, _baseProjectileCount + _additionalProjectileCount);
    }

    private void RecalculateProjectileSize()
    {
        ProjectileSize = Mathf.Max(0.1f, _baseProjectileSize + _additionalProjectileSize);
    }

    private void RecalculateProjectileSpeed()
    {
        ProjectileSpeed = Mathf.Max(0.1f, _baseProjectileSpeed + _additionalProjectileSpeed);
    }

    private void RecalculateProjectileRange()
    {
        ProjectileRange = Mathf.Max(0.1f, _baseProjectileRange + _additionalProjectileRange);
    }

    private void RecalculateReflectionCount()
    {
        ReflectionCount = Mathf.Max(0, _baseReflectionCount + _additionalReflectionCount);
    }

    private void RecalculatePenetrationCount()
    {
        PenetrationCount = Mathf.Max(0, _basePenetrationCount + _additionalPenetrationCount);
    }
    #endregion

    /// <summary>
    /// 모든 스탯을 한 번에 재계산하는 메서드
    /// </summary>
    private void RecalculateAllStats()
    {
        _suppressEvents = true;  // 이벤트 발생 억제 시작

        // 현재 체력 저장 (변경 사항 감지용)
        int oldHealth = _currentHealth;

        RecalculateMaxHealth();
        RecalculateAttackPower();
        RecalculateMoveSpeed();
        RecalculateAttackSpeed();
        RecalculateAutoFire();
        RecalculateSpreadAngle();
        RecalculateMultiAngle();
        RecalculateProjectileCount();
        RecalculateProjectileSize();
        RecalculateProjectileSpeed();
        RecalculateProjectileRange();
        RecalculateReflectionCount();
        RecalculatePenetrationCount();

        _suppressEvents = false;  // 이벤트 발생 억제 해제

        // 체력이 변경된 경우에만 이벤트 호출
        if (oldHealth != _currentHealth)
        {
            OnHealthChanged?.Invoke();
        }
    }

    private void Start()
    {
        RecalculateAllStats();
    }

    /// <summary>
    /// 에디터 상에서 스탯이 변경될 때 호출(테스트용)
    /// </summary>
    private void OnValidate()
    {
        // 에디터 모드에서만 처리
        if (!Application.isPlaying)
            return;

        // 현재 인스턴스가 정상적으로 초기화되었는지 확인
        if (Instance != this)
            return;

        // 모든 스탯 재계산 (이벤트 발생 최소화)
        RecalculateAllStats();
    }

    /// <summary>
    /// 모든 추가 스탯 초기화 메서드
    /// </summary>
    public void ResetAdditionalStats()
    {
        _suppressEvents = true;

        // 현재 체력 저장 (변경 사항 감지용)
        int oldHealth = _currentHealth;

        _additionalMaxHealth = 0;
        _additionalAttackPower = 0f;
        _additionalMoveSpeed = 0f;
        _additionalAttackSpeed = 0f;
        _additionalAutoFire = false;
        _additionalSpreadAngle = 0f;
        _additionalMultiAngle = 0f;
        _additionalProjectileCount = 0;
        _additionalProjectileSize = 0f;
        _additionalProjectileSpeed = 0f;
        _additionalProjectileRange = 0f;
        _additionalReflectionCount = 0;
        _additionalPenetrationCount = 0;

        _suppressEvents = false;

        RecalculateAllStats();

        // 체력 변경 확인 (RecalculateAllStats에서도 체크하지만, 중복 체크 방지를 위해 여기서는 호출하지 않음)
    }

    /// <summary>
    /// 여러 스탯을 한꺼번에 변경할 때 사용하는 메서드
    /// </summary>
    public void ModifyMultipleStats(Action modifyAction)
    {
        if (modifyAction == null) return;

        bool wasSuppressed = _suppressEvents; // 현재 억제 상태 저장
        _suppressEvents = true;

        // 현재 체력 저장 (변경 사항 감지용)
        int oldHealth = _currentHealth;

        try
        {
            modifyAction.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"StatHandler: ModifyMultipleStats 실행 중 예외 발생: {ex.Message}");
        }
        finally
        {
            _suppressEvents = wasSuppressed; // 예외가 발생해도 원래 상태로 복원

            // 체력이 변경되었고 이벤트 억제가 아닐 때만 이벤트 호출
            if (!_suppressEvents && oldHealth != _currentHealth)
            {
                OnHealthChanged?.Invoke();
            }
        }
    }

    public void ModifyStat(EItemType type, int val, float time, bool isPermanent)
    {
        //시간되면 변경
        switch (type)
        {
            case EItemType.Health:
                CurrentHealth += val;
                break;
            case EItemType.Speed:
                BaseMoveSpeed += val;
                if (!isPermanent)
                    StartCoroutine(TempStatForDuration(EItemType.Speed, val, time));
                break;
            case EItemType.Atk:
                BaseAttackPower += val;
                if (!isPermanent)
                    StartCoroutine(TempStatForDuration(EItemType.Atk, val, time));
                break;
            case EItemType.AtkSpeed:
                BaseAttackSpeed += val;
                if (!isPermanent)
                    StartCoroutine(TempStatForDuration(EItemType.AtkSpeed, val, time));
                break;
        }
    }

    private IEnumerator TempStatForDuration(EItemType type, int val, float duration) //duration 동안 일시적인 스탯
    {
        yield return new WaitForSeconds(duration);
        switch (type)
        {
            case EItemType.Speed:
                BaseMoveSpeed -= val;
                break;
            case EItemType.Atk:
                BaseAttackPower -= val;
                break;
            case EItemType.AtkSpeed:
                BaseAttackSpeed -= val;
                break;
        }
    }

    public void ModifyEquipStat(EquipItemData data)
    {
        if (data.canStack)
        {
            switch (data.type)
            {
                case EEquipItemType.MaxHealth:
                    if (data.curStack < data.maxStackAmount)
                    {
                        data.curStack++;
                        BaseMaxHealth += int.Parse(data.value.ToString());
                    }
                    break;
                case EEquipItemType.ProjectileCnt:
                    if (data.curStack < data.maxStackAmount)
                    {
                        data.curStack++;
                        BaseProjectileCount += (int)data.value;
                    }
                    break;
                case EEquipItemType.FireSpeed:
                    if (data.curStack < data.maxStackAmount)
                    {
                        data.curStack++;
                        BaseAttackSpeed += data.value;
                    }
                    break;
                case EEquipItemType.ProjectileSize:
                    if (data.curStack < data.maxStackAmount)
                    {
                        data.curStack++;
                        BaseProjectileSize += data.value;
                    }
                    break;
                case EEquipItemType.ProjectileSpeed:
                    if (data.curStack < data.maxStackAmount)
                    {
                        data.curStack++;
                        BaseProjectileSpeed += data.value;
                    }
                    break;
                case EEquipItemType.ReflectCnt:
                    if (data.curStack < data.maxStackAmount)
                    {
                        data.curStack++;
                        BaseReflectionCount += int.Parse(data.value.ToString());
                    }
                    break;
                case EEquipItemType.PenetrationCnt:
                    if (data.curStack < data.maxStackAmount)
                    {
                        data.curStack++;
                        BasePenetrationCount += int.Parse(data.value.ToString());
                    }
                    break;
            }
        }
    }
}