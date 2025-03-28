using UnityEngine;

public class FollowCamera2D : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

    [Header("Following Settings")]
    [SerializeField] private bool useFixedUpdate = true;
    [SerializeField] private bool instantFollow = false;
    [SerializeField] private float smoothSpeed = 10.0f;

    [Header("Mouse Influence Settings")]
    [SerializeField] private float mouseInfluence = 1.0f;
    [SerializeField] private float maxMouseDistance = 3.0f;
    [SerializeField] private float mouseSmoothSpeed = 3.0f;

    [Header("Distance Limit")]
    [SerializeField] private float maxDistanceFromTarget = 5.0f;
    private float maxDistanceSqr;

    private Vector3 smoothedMouseOffset;
    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private float zDistance;

    // 재사용 가능한 Vector2 변수들
    private Vector2 targetPos2D = Vector2.zero;
    private Vector2 cameraPos2D = Vector2.zero;
    private Vector2 mouseDirection = Vector2.zero;

    private void Awake()
    {
        maxDistanceSqr = maxDistanceFromTarget * maxDistanceFromTarget;
        zDistance = Mathf.Abs(offset.z);
    }

    private void Start()
    {
        // Player 태그를 가진 객체 찾기
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
            {
                Debug.LogError("No target found!");
                enabled = false;
                return;
            }
        }

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("No camera found!");
                enabled = false;
                return;
            }
        }

        transform.position = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            offset.z
        );
    }

    private void Update()
    {
        if (!useFixedUpdate)
        {
            UpdateCameraPosition(Time.deltaTime);
        }

        CalculateMouseInfluence(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            UpdateCameraPosition(Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// 카메라 위치 업데이트
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateCameraPosition(float deltaTime)
    {
        if (target == null) return;

        // 원하는 위치 계산
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x + smoothedMouseOffset.x,
            target.position.y + offset.y + smoothedMouseOffset.y,
            offset.z
        );

        // 현재 위치에서 목표 위치로 이동
        Vector3 newPosition = instantFollow ?
            desiredPosition :
            Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1.0f / smoothSpeed, Mathf.Infinity, deltaTime);

        // 최대 거리 체크
        targetPos2D.Set(target.position.x, target.position.y);
        cameraPos2D.Set(newPosition.x, newPosition.y);

        float sqrDist = (cameraPos2D - targetPos2D).sqrMagnitude;
        if (sqrDist > maxDistanceSqr)
        {
            Vector2 dirToTarget = (cameraPos2D - targetPos2D).normalized;
            Vector2 limitedPos = targetPos2D + dirToTarget * maxDistanceFromTarget;
            newPosition.x = limitedPos.x;
            newPosition.y = limitedPos.y;
        }

        transform.position = newPosition;
    }

    /// <summary>
    /// 마우스를 얼마나 따라갈지 계산
    /// </summary>
    /// <param name="deltaTime"></param>
    private void CalculateMouseInfluence(float deltaTime)
    {
        if (target == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = zDistance;
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mousePos);

        mouseDirection.x = mouseWorldPos.x - target.position.x;
        mouseDirection.y = mouseWorldPos.y - target.position.y;
        float magnitude = mouseDirection.magnitude;
        if (magnitude > 0.001f)
        {
            mouseDirection.x /= magnitude;
            mouseDirection.y /= magnitude;
        }

        Vector3 targetMouseOffset = new Vector3(
            mouseDirection.x * mouseInfluence,
            mouseDirection.y * mouseInfluence,
            0f
        );

        float targetOffsetSqrMag = targetMouseOffset.sqrMagnitude;
        float maxDistSqr = maxMouseDistance * maxMouseDistance;
        if (targetOffsetSqrMag > maxDistSqr)
        {
            float scaleFactor = maxMouseDistance / Mathf.Sqrt(targetOffsetSqrMag);
            targetMouseOffset.x *= scaleFactor;
            targetMouseOffset.y *= scaleFactor;
        }

        smoothedMouseOffset = Vector3.Lerp(
            smoothedMouseOffset,
            targetMouseOffset,
            mouseSmoothSpeed * deltaTime
        );
    }

    private void OnValidate()
    {
        maxDistanceSqr = maxDistanceFromTarget * maxDistanceFromTarget;
    }
}