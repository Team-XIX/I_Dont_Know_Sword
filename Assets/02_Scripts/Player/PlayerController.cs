using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("참조")]
    [SerializeField] private GameObject mainSprite;
    [SerializeField] private Transform weaponTransform;

    private Collider2D playerCollider; // 나중에 사용 예정(아마도)
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private StatHandler statHandler;

    private WaitForSeconds dashDurationWait;
    private WaitForSeconds dashCooldownWait;
    private WaitForSeconds invincibilityDurationWait;

    private int isMovingHash;
    private int isDashingHash;
    public bool IsInvincible => isInvincible;
    public bool IsDashing => isDashing;
    public Vector2 MoveDirection => lastMoveDirection;
    private Vector2 lastMoveDirection = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

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

        // 연사 모드이고 발사 버튼이 눌려있으면 자동 발사
        if (statHandler != null && statHandler.AutoFire && isFireButtonPressed && canFire && !isDashing)
        {
            FireProjectile();
            canFire = false; // 발사 후 쿨다운 시작
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
        // 버튼이 눌렸을 때
        if (context.performed)
        {
            isFireButtonPressed = true;

            // 대시 중이 아니고 발사 가능한 상태라면 즉시 발사
            if (!isDashing && canFire)
            {
                FireProjectile();
                canFire = false; // 발사 후 쿨다운 시작
            }
        }
        // 버튼이 떼어졌을 때
        else if (context.canceled)
        {
            isFireButtonPressed = false;
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
            // AttackSpeed 1 = 1초당 1번 발사, AttackSpeed 2 = 1초당 2번 발사, AttackSpeed 3 = 1초당 3번 발사
            fireInterval = 1f / statHandler.AttackSpeed;
        }
        else
        {
            fireInterval = 1f; // 기본값
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
    /// 플레이어가 발사체를 발사
    /// </summary>
    private void FireProjectile()
    {
        if (ProjectileSystem.Instance == null) return;

        // 발사 간격 업데이트 - StatHandler의 값이 변경되었을 수 있으므로 발사 시마다 확인
        UpdateFireInterval();

        Vector2 mousePosition = Vector2.zero;
        Vector2 fireDirection = lastMoveDirection;

        WeaponFloating weaponFloating = weaponTransform != null ?
            weaponTransform.GetComponent<WeaponFloating>() : null;

        if (weaponFloating != null)
        {
            mousePosition = weaponFloating.GetMousePosition();
            fireDirection = (mousePosition - (Vector2)transform.position).normalized;
        }

        Vector2 firePosition;
        float weaponOffset = 1.0f;

        if (weaponTransform != null)
        {
            firePosition = weaponTransform.position;
        }
        else
        {
            firePosition = (Vector2)transform.position + fireDirection * weaponOffset;
        }

        ProjectileSystem.Instance.FireProjectile(firePosition, fireDirection);
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

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        if (statHandler != null)
        {
            statHandler.CurrentHealth -= damage;
        }
    }
}