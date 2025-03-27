using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("대시 설정")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashInvincibilityDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    // 참조용 컴포넌트
    private Collider2D playerCollider;

    // 성능 최적화를 위한 캐싱
    private WaitForSeconds dashDurationWait;
    private WaitForSeconds dashCooldownWait;
    private WaitForSeconds invincibilityDurationWait;

    void Awake()
    {
        // 컴포넌트 캐싱 (GetComponent 호출 최소화)
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // WaitForSeconds 캐싱 (가비지 컬렉션 최소화)
        dashDurationWait = new WaitForSeconds(dashDuration);
        dashCooldownWait = new WaitForSeconds(dashCooldown);
        invincibilityDurationWait = new WaitForSeconds(dashInvincibilityDuration);
    }

    void FixedUpdate()
    {
        // 물리 기반 이동은 FixedUpdate에서 처리 (성능 최적화)
        if (!isDashing)
        {
            ApplyMovement();
        }
    }

    #region 입력 처리

    // Input System에서 이동 입력을 받는 콜백 메서드
    public void OnMove(InputAction.CallbackContext context)
    {
        // 입력 데이터만 저장 (실제 처리는 FixedUpdate에서)
        moveInput = context.ReadValue<Vector2>();
    }

    // Input System에서 대시 입력을 받는 콜백 메서드
    public void OnDash(InputAction.CallbackContext context)
    {
        // performed 단계에서만 실행 (버튼 누를 때)
        if (context.performed && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    // Input System에서 발사 입력을 받는 콜백 메서드
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FireProjectile();
        }
    }

    #endregion

    #region 이동/액션 구현

    // 실제 이동을 적용하는 메서드 (FixedUpdate에서 호출)
    private void ApplyMovement()
    {
        // 외부 데이터에서 이동속도를 가져올 수 있도록 설계
        // 예: float currentMoveSpeed = PlayerDataManager.Instance.GetStat(StatType.MoveSpeed);

        // 정규화된 방향에 속도 곱하기 (대각선 이동 보정)
        if (moveInput.sqrMagnitude > 0)
        {
            Vector2 movementVelocity = moveInput.normalized * moveSpeed;
            rb.velocity = movementVelocity;
        }
        else
        {
            // 입력이 없을 때 정지
            rb.velocity = Vector2.zero;
        }
    }

    // 대시 기능 구현
    private IEnumerator PerformDash()
    {
        // 대시 상태 설정
        canDash = false;
        isDashing = true;

        // 대시 방향 결정 (입력이 없으면 캐릭터가 바라보는 방향)
        Vector2 dashDirection = moveInput.normalized;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = transform.right;
        }

        // 대시 속도 적용 (물리 기반)
        rb.velocity = dashDirection * dashForce;

        // 무적 효과 적용
        StartCoroutine(ApplyInvincibility());

        // 대시 지속 시간 (캐싱된 WaitForSeconds 사용)
        yield return dashDurationWait;

        // 대시 종료 후 속도 초기화
        isDashing = false;
        rb.velocity = Vector2.zero;

        // 대시 쿨다운 적용 (캐싱된 WaitForSeconds 사용)
        yield return dashCooldownWait;
        canDash = true;
    }

    // 무적 효과 적용
    private IEnumerator ApplyInvincibility()
    {
        // 콜라이더 비활성화로 무적 구현
        playerCollider.enabled = false;

        // 캐싱된 WaitForSeconds 사용
        yield return invincibilityDurationWait;

        // 무적 해제
        playerCollider.enabled = true;
    }

    // 투사체 발사 기능 (인터페이스 연결용)
    private void FireProjectile()
    {
        // 투사체 발사 시스템과 연결할 인터페이스 메서드
        // 구현은 다른 스크립트에서 할 예정

        // 예시: ProjectileSystem.Instance.FireProjectile(transform.position, aimDirection);
    }

    #endregion
}