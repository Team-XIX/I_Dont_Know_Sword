using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("대시 설정")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashInvincibilityDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.45f;
    private bool canDash = true;
    private bool isDashing = false;
    private bool isInvincible = false;

    [Header("발사 설정")]
    private float fireTimer = 0f;
    private float fireInterval = 1f; // 기본값 (StatHandler에서 AttackSpeed 값으로 대체됨)
    private bool canFire = true;
    private bool isFireButtonPressed = false; // 발사 버튼 누름 상태 추적

    [Header("피격 설정")]
    [SerializeField] private float damageInvincibilityDuration = 1.0f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("참조")]
    [SerializeField] private GameObject mainSprite;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private WeaponManager weaponManager;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private StatHandler statHandler;

    private WaitForSeconds dashDurationWait;
    private WaitForSeconds dashCooldownWait;
    private WaitForSeconds invincibilityDurationWait;
    private WaitForSeconds damageInvincibilityWait;
    private WaitForSeconds blinkIntervalWait;

    private int isMovingHash;
    private int isDashingHash;
    private int isHitHash;
    public bool IsInvincible => isInvincible;
    public bool IsDashing => isDashing;
    public Vector2 MoveDirection => lastMoveDirection;
    private Vector2 lastMoveDirection = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (mainSprite == null)
        {
            mainSprite = transform.Find("MainSprite").gameObject;
        }

        animator = mainSprite.GetComponent<Animator>();
        spriteRenderer = mainSprite.GetComponent<SpriteRenderer>();

        dashDurationWait = new WaitForSeconds(dashDuration);
        dashCooldownWait = new WaitForSeconds(dashCooldown);
        invincibilityDurationWait = new WaitForSeconds(dashInvincibilityDuration);

        isMovingHash = Animator.StringToHash("isMoving");
        isDashingHash = Animator.StringToHash("isDashing");

        if (weaponTransform == null)
        {
            weaponTransform = transform.Find("Weapon");
        }
    }

    void Start()
    {
        statHandler = StatHandler.Instance;
        if (statHandler != null)
        {
            moveSpeed = statHandler.MoveSpeed;
            UpdateFireInterval();
        }
    }

    void Update()
    {
        UpdateAnimationState();
        UpdateFireTimer();

        // 연사 모드이고 좌클릭 유지하면 자동 발사
        if (statHandler != null && statHandler.AutoFire && isFireButtonPressed && canFire && !isDashing)
        {
            FireProjectile();
            canFire = false;
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            ApplyMovement();
        }
    }

    #region 입력 처리

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput.normalized;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isFireButtonPressed = true;

            if (!isDashing && canFire)
            {
                FireProjectile();
                canFire = false;
            }
        }
        else if (context.canceled)
        {
            isFireButtonPressed = false;
        }
    }

    public void OnSwitchWeaponPrevious(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing)
        {
            SwitchToPreviousWeapon();
        }
    }

    public void OnSwitchWeaponNext(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing)
        {
            SwitchToNextWeapon();
        }
    }

    #endregion

    /// <summary>
    /// 발사 시간 업데이트 및 쿨다운 관리
    /// </summary>
    private void UpdateFireTimer()
    {
        if (!canFire)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                fireTimer = 0f;
                canFire = true;
            }
        }
    }

    /// <summary>
    /// StatHandler의 AttackSpeed 값에 따라 발사 간격 업데이트
    /// </summary>
    private void UpdateFireInterval()
    {
        if (statHandler != null)
        {
            // AttackSpeed 값이 클수록 발사 간격이 짧아짐
            // AttackSpeed 1 = 1초당 1번 발사, AttackSpeed 2 = 1초당 2번 발사, AttackSpeed 3 = 1초당 3번 발사...
            fireInterval = 1f / statHandler.AttackSpeed;
        }
        else
        {
            fireInterval = 1f;
        }
    }

    /// <summary>
    /// 플레이어 이동 실행
    /// </summary>
    private void ApplyMovement()
    {
        float currentMoveSpeed = statHandler != null ? statHandler.MoveSpeed : moveSpeed;

        if (moveInput != Vector2.zero)
        {
            Vector2 movementVelocity = moveInput.normalized * currentMoveSpeed;
            rb.velocity = movementVelocity;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// 대시 실행
    /// </summary>
    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        if (animator != null)
        {
            animator.SetBool(isDashingHash, true);
        }

        Vector2 dashDirection = moveInput != Vector2.zero ? moveInput.normalized : lastMoveDirection;

        if (statHandler != null)
        {
            dashForce = statHandler.MoveSpeed * 1.5f;
        }
        else
        {
            dashForce = moveSpeed * 1.5f;
        }

        rb.velocity = dashDirection * dashForce;
        StartCoroutine(ApplyInvincibility());
        yield return dashDurationWait;

        isDashing = false;
        rb.velocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool(isDashingHash, false);
        }

        yield return dashCooldownWait;
        canDash = true;
    }

    /// <summary>
    /// 대시 중 무적 상태 적용
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyInvincibility()
    {
        isInvincible = true;
        yield return invincibilityDurationWait;
        isInvincible = false;
    }

    /// <summary>
    /// 플레이어가 투사체 발사
    /// </summary>
    private void FireProjectile()
    {
        if (ProjectileSystem.Instance == null) return;

        UpdateFireInterval();

        Vector2 mousePosition = Vector2.zero;
        Vector2 fireDirection = lastMoveDirection;

        WeaponFloating weaponFloating = weaponTransform != null ?
            weaponTransform.GetComponent<WeaponFloating>() : null;

        if (weaponFloating != null)
        {
            mousePosition = weaponFloating.GetMousePosition();
            fireDirection = (mousePosition - (Vector2)transform.position).normalized;

            Vector2 targetWeaponPosition = (Vector2)transform.position +
                fireDirection * weaponFloating.GetFireDistance();

            ProjectileSystem.Instance.FireProjectile(targetWeaponPosition, fireDirection);
        }
    }

    /// <summary>
    /// 애니메이션 상태 업데이트
    /// </summary>
    private void UpdateAnimationState()
    {
        if (animator == null || mainSprite == null) return;

        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool(isMovingHash, isMoving && !isDashing);

        if (!isDashing && isMoving)
        {
            bool facingRight = moveInput.x > 0;
            UpdateFacingDirection(facingRight);
        }
    }

    /// <summary>
    /// 플레이어의 바라보는 방향을 업데이트
    /// </summary>
    /// <param name="facingRight"></param>
    private void UpdateFacingDirection(bool facingRight)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }
        else
        {
            Vector3 scale = mainSprite.transform.localScale;
            float xScale = Mathf.Abs(scale.x);
            mainSprite.transform.localScale = new Vector3(
                facingRight ? xScale : -xScale,
                scale.y,
                scale.z
            );
        }
    }

    /// <summary>
    /// 플레이어가 데미지를 받을 때 호출되는 함수
    /// </summary>
    /// <param name="damage">받는 데미지 양</param>
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        if (statHandler != null)
        {
            statHandler.CurrentHealth -= damage;
        }

        //// 데미지 효과 애니메이션 재생 (넣을수도)
        //if (animator != null)
        //{
        //    animator.SetTrigger(isHitHash);
        //}

        StartCoroutine(DamageInvincibility());
    }

    /// <summary>
    /// 데미지를 받은 후 일시적인 무적 상태 및 깜빡임 효과 적용
    /// </summary>
    private IEnumerator DamageInvincibility()
    {
        isInvincible = true;

        // 깜빡임 효과
        StartCoroutine(BlinkEffect());
        yield return damageInvincibilityWait;
        isInvincible = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }

    /// <summary>
    /// 데미지를 받은 후 깜빡임 효과 적용
    /// </summary>
    private IEnumerator BlinkEffect()
    {
        if (spriteRenderer == null) yield break;

        float endTime = Time.time + damageInvincibilityDuration;

        while (Time.time < endTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return blinkIntervalWait;
        }

        spriteRenderer.enabled = true;
    }

    /// <summary>
    /// 이전 무기로 전환
    /// </summary>
    private void SwitchToPreviousWeapon()
    {
        if (weaponManager != null)
        {
            weaponManager.SwitchToPreviousWeapon();
        }
    }

    /// <summary>
    /// 다음 무기로 전환
    /// </summary>
    private void SwitchToNextWeapon()
    {
        if (weaponManager != null)
        {
            weaponManager.SwitchToNextWeapon();
        }
    }
    public void UseItem(Item item)// Add Item
    {
        ItemData data = item.itemData;

        if (data == null)
        {
            Debug.Log("ItemData Null");
        }

        statHandler.ModifyStat(data.type, data.value, data.time, data.isPermanent);
    }

    public void AddItem(EquipItem equipItem) // Add EquipItem
    {
        EquipItemData data = equipItem.equipItemData;

        if (data == null)
        {
            Debug.Log("EquipItemData Null");
        }

        //statHandler.ModifyEquipStat(data.type,data.value,data.maxStackAmount,data.canStack);
        statHandler.ModifyEquipStat(data);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Item Check
        if (collision.gameObject.TryGetComponent<Item>(out Item item))
        {
            UseItem(item);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.TryGetComponent<EquipItem>(out EquipItem equipItem))
        {
            AddItem(equipItem);
            Destroy(collision.gameObject);
        }
    }
}