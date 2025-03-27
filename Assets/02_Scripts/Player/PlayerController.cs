using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("��� ����")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashInvincibilityDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.45f;
    private bool canDash = true;
    private bool isDashing = false;
    private bool isInvincible = false;

    [Header("����")]
    [SerializeField] private GameObject mainSprite;

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

    #region �Է� ó��

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
    /// �÷��̾� �̵� ����
    /// </summary>
    private void ApplyMovement()
    {
        // Player ������ ��ٸ�����...
        float currentMoveSpeed = moveSpeed;

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
    /// ��� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        if (animator != null)
        {
            animator.SetBool(isDashingHash, true);
        }

        Vector2 dashDirection = moveInput != Vector2.zero ? moveInput.normalized : lastMoveDirection;

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
    /// ��� �� ���� ���� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyInvincibility()
    {
        isInvincible = true;
        playerCollider.enabled = false;
        yield return invincibilityDurationWait;
        playerCollider.enabled = true;
        isInvincible = false;
    }

    /// <summary>
    /// �÷��̾ �߻�ü�� �߻�
    /// </summary>
    private void FireProjectile()
    {
        // ���� ����
        // ProjectileSystem.Instance.FireProjectile(transform.position, lastMoveDirection);
    }

    /// <summary>
    /// �ִϸ��̼� ���� ������Ʈ
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
    /// �÷��̾��� �ٶ󺸴� ������ ������Ʈ
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
}