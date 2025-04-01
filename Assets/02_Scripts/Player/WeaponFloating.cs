using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponFloating : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera mainCamera;

    [Header("위치 설정")]
    [SerializeField] private float fixedFireDistance = 1.2f; // 플레이어로부터 떨어진 거리

    [Header("보간 설정")]
    [SerializeField] private float positionLerpSpeed = 12.0f; // 위치 보간 속도
    [SerializeField] private float rotationLerpSpeed = 15.0f; // 회전 보간 속도

    private Vector2 mousePosition;
    private Vector2 fireDirection;
    private SpriteRenderer weaponSpriteRenderer;
    private Transform cachedTransform;

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
    }

    void Update()
    {
        UpdateMousePosition();
        HandleWeaponPosition();
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
    /// 무기 위치 처리 (항상 발사 상태로 동작)
    /// </summary>
    private void HandleWeaponPosition()
    {
        if (player == null) return;

        Vector2 playerPos = player.transform.position;
        fireDirection = (mousePosition - playerPos).normalized;

        Vector3 targetLocalPosition = new Vector3(
            fireDirection.x * fixedFireDistance,
            fireDirection.y * fixedFireDistance,
            0
        );

        cachedTransform.localPosition = Vector3.Lerp(
            cachedTransform.localPosition,
            targetLocalPosition,
            positionLerpSpeed * Time.deltaTime
        );

        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        cachedTransform.rotation = Quaternion.Slerp(
            cachedTransform.rotation,
            targetRotation,
            rotationLerpSpeed * Time.deltaTime
        );

        bool mouseIsRight = fireDirection.x >= 0;
        if (weaponSpriteRenderer != null)
        {
            weaponSpriteRenderer.flipX = false;
            weaponSpriteRenderer.flipY = !mouseIsRight;
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

    /// <summary>
    /// 무기가 플레이어로부터 떨어져야 하는 거리 반환
    /// </summary>
    public float GetFireDistance()
    {
        return fixedFireDistance;
    }
}