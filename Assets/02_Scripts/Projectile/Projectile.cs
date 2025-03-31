using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // 투사체가 비활성화될 때 호출되는 이벤트 (오브젝트 풀로 반환)
    public event Action<Projectile> OnDeactivate;

    private int wallLayer;
    private int obstacleLayer;
    private int enemyLayer;

    [Header("투사체 속성")]
    private float damage;
    private int roundedDamage;
    private float speed;
    private float lifetime;
    private int penetrationCount;
    private int reflectionCount;

    private Vector2 direction;
    private Rigidbody2D rb;
    private float timer;
    private List<Collider2D> hitTargets = new List<Collider2D>();
    private bool isInitialized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        wallLayer = LayerMask.NameToLayer("Wall");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
        enemyLayer = LayerMask.NameToLayer("Enemy"); // Enemy 레이어 캐싱
    }

    private void OnEnable()
    {
        timer = 0f;
        hitTargets.Clear();
    }

    /// <summary>
    /// 투사체 초기화
    /// </summary>
    public void Initialize(float damage, Vector2 direction, float speed, float lifetime, float size, int penetrationCount, int reflectionCount)
    {
        this.damage = damage;
        this.roundedDamage = Mathf.RoundToInt(damage); // 데미지 정수형으로 변환(필요 없어지면 없애야지)
        this.direction = direction.normalized;
        this.speed = speed;
        this.lifetime = lifetime;
        this.penetrationCount = penetrationCount;
        this.reflectionCount = reflectionCount;

        // 크기 조정
        transform.localScale = Vector3.one * size;

        // 방향에 맞게 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 초기 속도 설정
        rb.velocity = this.direction * this.speed;

        timer = 0f;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        // 수명 체크
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized)
            return;

        // 벽에 부딪힌 경우 반사 처리
        if (other.gameObject.layer == wallLayer || other.gameObject.layer == obstacleLayer)
        {
            HandleReflection(other);
            return;
        }

        // Enemy 레이어를 가진 IDamageable 오브젝트와의 충돌 처리
        if (other.gameObject.layer == enemyLayer)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && !hitTargets.Contains(other))
            {
                // 데미지 적용 (정수형 데미지 사용)
                damageable.TakeDamage(roundedDamage);
                hitTargets.Add(other);

                // 관통 처리
                HandlePenetration();
            }
        }
    }

    /// <summary>
    /// 투사체 관통 처리
    /// </summary>
    private void HandlePenetration()
    {
        if (penetrationCount > 0)
        {
            penetrationCount--;
        }
        else
        {
            Deactivate();
        }
    }

    /// <summary>
    /// 투사체 반사 처리
    /// </summary>
    private void HandleReflection(Collider2D other)
    {
        if (reflectionCount <= 0)
        {
            Deactivate();
            return;
        }

        // 반사 횟수 감소
        reflectionCount--;

        Vector2 hitPoint = other.ClosestPoint(transform.position);
        Vector2 normal = ((Vector2)transform.position - hitPoint).normalized;

        // 반사 방향 계산
        direction = Vector2.Reflect(direction, normal);

        // 속도 적용
        rb.velocity = direction * speed;

        // 방향에 맞게 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// 투사체를 비활성화하고 풀로 반환
    /// </summary>
    private void Deactivate()
    {
        if (gameObject.activeInHierarchy)
        {
            isInitialized = false;
            rb.velocity = Vector2.zero;
            OnDeactivate?.Invoke(this);
        }
    }

    /// <summary>
    /// 게임 오브젝트 파괴될 때 호출
    /// </summary>
    private void OnDestroy()
    {
        OnDeactivate = null;
    }
}