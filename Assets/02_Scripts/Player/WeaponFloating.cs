using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class WeaponFloating : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera mainCamera;

    [Header("위치 설정")]
    [SerializeField] private float horizontalOffset = 1f;     // 플레이어로부터 좌우 거리(기본 상태에서 플레이어랑 떨어진 거리)
    [SerializeField] private float fixedFireDistance = 1.2f;    // 발사 상태에서 플레이어로부터 떨어진 거리
    [SerializeField] private float verticalOffset = 0.0f;       // 플레이어로부터 수직 위치 보정(0으로 두는게 기본값. 제일 보기 좋음)

    [Header("Tween 설정")]
    [SerializeField] private float floatDuration = 1.5f;        // 위아래 움직임의 주기
    [SerializeField] private float floatDistance = 0.1f;        // 위아래 움직이는 거리 (작게 설정)
    [SerializeField] private float positionLerpSpeed = 12.0f;   // 위치 보간 속도
    [SerializeField] private float rotationLerpSpeed = 15.0f;   // 회전 보간 속도
    [SerializeField] private float fireStateDuration = 1.0f;    // 발사 상태 지속 시간

    private bool isInFireState = false;
    private Vector2 mousePosition;
    private Vector2 fireDirection;
    private Coroutine fireStateCoroutine;
    private bool isFacingRight = true;
    private SpriteRenderer weaponSpriteRenderer;
    private Sequence floatSequence;
    private Transform parentTransform;
    private Transform cachedTransform;

    // 외부에서 접근 가능한 속성들
    public bool IsInFireState => isInFireState;
    public Vector2 FireDirection => fireDirection;

    void Start()
    {
        cachedTransform = transform;

        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
            if (player != null && cachedTransform.parent != player.transform)
            {
                cachedTransform.SetParent(player.transform);
            }
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        parentTransform = cachedTransform.parent;

        SetInitialPosition();
        SetupFloatAnimation();
    }

    void Update()
    {
        UpdateMousePosition();

        if (!isInFireState)
        {
            HandleNormalState();
        }
        else
        {
            HandleFireState();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnFireInput();
        }
    }

    /// <summary>
    /// 마우스 위치를 월드 좌표로 업데이트
    /// </summary>
    private void UpdateMousePosition()
    {
        Vector2 screenMousePos = Mouse.current.position.ReadValue();
        mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, 0));
    }

    /// <summary>
    /// 초기 위치 설정
    /// </summary>
    private void SetInitialPosition()
    {
        if (player == null) return;

        float xOffset = isFacingRight ? horizontalOffset : -horizontalOffset;
        cachedTransform.localPosition = new Vector3(xOffset, verticalOffset, 0);
    }

    /// <summary>
    /// 일반 상태 처리
    /// </summary>
    private void HandleNormalState()
    {
        if (player == null) return;

        Vector2 playerDirection = player.MoveDirection;

        if (playerDirection.x != 0)
        {
            isFacingRight = playerDirection.x > 0;
        }

        float xOffset = isFacingRight ? horizontalOffset : -horizontalOffset;
        Vector3 targetLocalPosition = new Vector3(xOffset, verticalOffset, 0);

        cachedTransform.localPosition = Vector3.Lerp(
            cachedTransform.localPosition,
            targetLocalPosition,
            positionLerpSpeed * Time.deltaTime
        );

        cachedTransform.localRotation = Quaternion.identity;

        if (weaponSpriteRenderer != null)
        {
            weaponSpriteRenderer.flipX = !isFacingRight;
            weaponSpriteRenderer.flipY = false;
        }

        if (floatSequence != null && !floatSequence.IsPlaying())
        {
            floatSequence.Play();
        }
    }

    /// <summary>
    /// 발사 상태 처리
    /// </summary>
    private void HandleFireState()
    {
        if (player == null) return;

        if (floatSequence != null && floatSequence.IsPlaying())
        {
            floatSequence.Pause();
        }

        // 마우스 방향 계산
        Vector2 playerPos = player.transform.position;
        fireDirection = (mousePosition - playerPos).normalized;

        // 로컬 좌표에서 처리하여 플레이어의 자식으로 유지
        Vector3 targetLocalPosition = new Vector3(
            fireDirection.x * fixedFireDistance,
            fireDirection.y * fixedFireDistance + verticalOffset,
            0
        );

        cachedTransform.localPosition = Vector3.Lerp(
            cachedTransform.localPosition,
            targetLocalPosition,
            positionLerpSpeed * Time.deltaTime
        );

        // 회전 적용
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        cachedTransform.rotation = Quaternion.Slerp(
            cachedTransform.rotation,
            targetRotation,
            rotationLerpSpeed * Time.deltaTime
        );

        // 스프라이트 방향 설정
        bool mouseIsRight = fireDirection.x >= 0;
        if (weaponSpriteRenderer != null)
        {
            weaponSpriteRenderer.flipX = false;
            weaponSpriteRenderer.flipY = !mouseIsRight;
        }
    }

    /// <summary>
    /// 발사 입력 처리 (좌클릭)
    /// </summary>
    public void OnFireInput()
    {
        if (player == null) return;

        // 마우스-플레이어 방향 계산
        fireDirection = (mousePosition - (Vector2)player.transform.position).normalized;

        if (fireStateCoroutine != null)
        {
            StopCoroutine(fireStateCoroutine);
        }

        fireStateCoroutine = StartCoroutine(FireStateCoroutine());
    }

    /// <summary>
    /// Fire 상태 처리 코루틴
    /// </summary>
    private IEnumerator FireStateCoroutine()
    {
        isInFireState = true;

        yield return new WaitForSeconds(fireStateDuration);

        isInFireState = false;
        fireStateCoroutine = null;
    }

    /// <summary>
    /// 총기가 위아래로 떠다니는 애니메이션
    /// </summary>
    private void SetupFloatAnimation()
    {
        if (floatSequence != null)
        {
            floatSequence.Kill();
        }

        floatSequence = DOTween.Sequence();

        floatSequence.Append(cachedTransform
            .DOLocalMoveY(verticalOffset + floatDistance, floatDuration / 2)
            .SetEase(Ease.InOutSine));

        floatSequence.Append(cachedTransform
            .DOLocalMoveY(verticalOffset - floatDistance, floatDuration / 2)
            .SetEase(Ease.InOutSine));

        floatSequence.SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// PlayerController의 발사(OnFire)랑 연결
    /// </summary>
    public void OnFireAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnFireInput();
        }
    }

    /// <summary>
    /// 오브젝트 파괴 시 DOTween 시퀀스 죽이기
    /// </summary>
    private void OnDestroy()
    {
        if (floatSequence != null)
        {
            floatSequence.Kill();
        }
    }

    /// <summary>
    /// 스크립트가 비활성화될 때 DOTween 시퀀스 일시정지
    /// </summary>
    private void OnDisable()
    {
        if (floatSequence != null)
        {
            floatSequence.Pause();
        }
    }

    /// <summary>
    /// 스크립트가 활성화될 때 DOTween 시퀀스 재시작
    /// </summary>
    private void OnEnable()
    {
        if (floatSequence != null)
        {
            floatSequence.Play();
        }
    }

    /// <summary>
    /// 현재 마우스 위치 반환 (월드 좌표)
    /// </summary>
    public Vector2 GetMousePosition()
    {
        return mousePosition;
    }

    /// <summary>
    /// 플레이어로부터 무기까지의 방향과 거리 계산
    /// </summary>
    public Vector2 GetWeaponDirection()
    {
        if (player == null) return Vector2.right;

        return ((Vector2)transform.position - (Vector2)player.transform.position).normalized;
    }

    /// <summary>
    /// 플레이어로부터 무기까지의 거리 반환
    /// </summary>
    public float GetWeaponDistance()
    {
        if (player == null) return fixedFireDistance;

        return Vector2.Distance(transform.position, player.transform.position);
    }
}