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
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashInvincibilityDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    // ������ ������Ʈ
    private Collider2D playerCollider;

    // ���� ����ȭ�� ���� ĳ��
    private WaitForSeconds dashDurationWait;
    private WaitForSeconds dashCooldownWait;
    private WaitForSeconds invincibilityDurationWait;

    void Awake()
    {
        // ������Ʈ ĳ�� (GetComponent ȣ�� �ּ�ȭ)
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // WaitForSeconds ĳ�� (������ �÷��� �ּ�ȭ)
        dashDurationWait = new WaitForSeconds(dashDuration);
        dashCooldownWait = new WaitForSeconds(dashCooldown);
        invincibilityDurationWait = new WaitForSeconds(dashInvincibilityDuration);
    }

    void FixedUpdate()
    {
        // ���� ��� �̵��� FixedUpdate���� ó�� (���� ����ȭ)
        if (!isDashing)
        {
            ApplyMovement();
        }
    }

    #region �Է� ó��

    // Input System���� �̵� �Է��� �޴� �ݹ� �޼���
    public void OnMove(InputAction.CallbackContext context)
    {
        // �Է� �����͸� ���� (���� ó���� FixedUpdate����)
        moveInput = context.ReadValue<Vector2>();
    }

    // Input System���� ��� �Է��� �޴� �ݹ� �޼���
    public void OnDash(InputAction.CallbackContext context)
    {
        // performed �ܰ迡���� ���� (��ư ���� ��)
        if (context.performed && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    // Input System���� �߻� �Է��� �޴� �ݹ� �޼���
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FireProjectile();
        }
    }

    #endregion

    #region �̵�/�׼� ����

    // ���� �̵��� �����ϴ� �޼��� (FixedUpdate���� ȣ��)
    private void ApplyMovement()
    {
        // �ܺ� �����Ϳ��� �̵��ӵ��� ������ �� �ֵ��� ����
        // ��: float currentMoveSpeed = PlayerDataManager.Instance.GetStat(StatType.MoveSpeed);

        // ����ȭ�� ���⿡ �ӵ� ���ϱ� (�밢�� �̵� ����)
        if (moveInput.sqrMagnitude > 0)
        {
            Vector2 movementVelocity = moveInput.normalized * moveSpeed;
            rb.velocity = movementVelocity;
        }
        else
        {
            // �Է��� ���� �� ����
            rb.velocity = Vector2.zero;
        }
    }

    // ��� ��� ����
    private IEnumerator PerformDash()
    {
        // ��� ���� ����
        canDash = false;
        isDashing = true;

        // ��� ���� ���� (�Է��� ������ ĳ���Ͱ� �ٶ󺸴� ����)
        Vector2 dashDirection = moveInput.normalized;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = transform.right;
        }

        // ��� �ӵ� ���� (���� ���)
        rb.velocity = dashDirection * dashForce;

        // ���� ȿ�� ����
        StartCoroutine(ApplyInvincibility());

        // ��� ���� �ð� (ĳ�̵� WaitForSeconds ���)
        yield return dashDurationWait;

        // ��� ���� �� �ӵ� �ʱ�ȭ
        isDashing = false;
        rb.velocity = Vector2.zero;

        // ��� ��ٿ� ���� (ĳ�̵� WaitForSeconds ���)
        yield return dashCooldownWait;
        canDash = true;
    }

    // ���� ȿ�� ����
    private IEnumerator ApplyInvincibility()
    {
        // �ݶ��̴� ��Ȱ��ȭ�� ���� ����
        playerCollider.enabled = false;

        // ĳ�̵� WaitForSeconds ���
        yield return invincibilityDurationWait;

        // ���� ����
        playerCollider.enabled = true;
    }

    // ����ü �߻� ��� (�������̽� �����)
    private void FireProjectile()
    {
        // ����ü �߻� �ý��۰� ������ �������̽� �޼���
        // ������ �ٸ� ��ũ��Ʈ���� �� ����

        // ����: ProjectileSystem.Instance.FireProjectile(transform.position, aimDirection);
    }

    #endregion
}