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

    [Header("참조")]
    [SerializeField] private GameObject mainSprite;
    [SerializeField] private Transform weaponTransform;

    private Collider2D playerCollider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

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
        if (StatHandler.Instance != null)
        {
            moveSpeed = StatHandler.Instance.MoveSpeed;
        }
    }

    void Update()
    {
        UpdateAnimationState();
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
        if (context.performed && !isDashing)
        {
            FireProjectile();
        }
    }

    #endregion


    /// <summary>
    /// 플레이어 이동 실행
    /// </summary>
    private void ApplyMovement()
    {
        float currentMoveSpeed = StatHandler.Instance != null ? StatHandler.Instance.MoveSpeed : moveSpeed;

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

        dashForce = StatHandler.Instance != null ? StatHandler.Instance.MoveSpeed * 1.5f : moveSpeed * 1.5f;
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
        if (StatHandler.Instance != null)
        {
            StatHandler.Instance.CurrentHealth -= damage;
        }
    }
}