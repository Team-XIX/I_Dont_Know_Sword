using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
    #region 싱글톤 구현
    public static ProjectileSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 오브젝트 풀 초기화
        InitializeObjectPool();
    }
    #endregion

    [Header("투사체 설정")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileParent;

    [Header("오브젝트 풀 설정")]
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private int maxPoolSize = 100;
    [SerializeField] private bool expandPoolIfNeeded = true;

    private Queue<Projectile> projectilePool = new Queue<Projectile>();
    private List<Projectile> activeProjectiles = new List<Projectile>();

    /// <summary>
    /// 오브젝트 풀 초기화
    /// </summary>
    private void InitializeObjectPool()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        // 부모 트랜스폼이 없다면 생성
        if (projectileParent == null)
        {
            GameObject parent = new GameObject("ProjectilePool");
            parent.transform.SetParent(transform);
            projectileParent = parent.transform;
        }

        // 초기 풀 사이즈만큼 투사체 생성
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateProjectileForPool();
        }
    }

    /// <summary>
    /// 풀에 새 투사체를 생성
    /// </summary>
    private Projectile CreateProjectileForPool()
    {
        GameObject obj = Instantiate(projectilePrefab, projectileParent);
        obj.SetActive(false);

        Projectile projectile = obj.GetComponent<Projectile>();
        if (projectile == null)
        {
            Destroy(obj);
            return null;
        }

        // 투사체에 반환 콜백 등록
        projectile.OnDeactivate += ReturnProjectileToPool;

        // 풀에 추가
        projectilePool.Enqueue(projectile);

        return projectile;
    }

    /// <summary>
    /// 투사체를 풀에서 가져옴
    /// </summary>
    private Projectile GetProjectileFromPool()
    {
        if (projectilePool.Count == 0)
        {
            if (expandPoolIfNeeded && activeProjectiles.Count < maxPoolSize)
            {
                return CreateProjectileForPool();
            }

            return null;
        }

        return projectilePool.Dequeue();
    }

    /// <summary>
    /// 투사체를 다시 풀 반환
    /// </summary>
    private void ReturnProjectileToPool(Projectile projectile)
    {
        // 활성 목록에서 제거
        activeProjectiles.Remove(projectile);

        // 이미 파괴된 오브젝트인지 확인
        if (projectile == null || projectile.gameObject == null)
            return;

        // 투사체 비활성화 및 초기화
        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(projectileParent);

        // 풀에 다시 추가
        projectilePool.Enqueue(projectile);
    }

    /// <summary>
    /// 투사체를 발사
    /// </summary>
    /// <param name="position">발사 시작 위치</param>
    /// <param name="direction">발사 방향</param>
    /// <param name="customDamage">커스텀 데미지 (null이면 StatHandler의 AttackPower 사용)</param>
    /// <param name="customOptions">커스텀 투사체 옵션 (null이면 StatHandler의 설정 사용)</param>
    public void FireProjectile(Vector2 position, Vector2 direction, float? customDamage = null, ProjectileOptions customOptions = null)
    {
        // StatHandler에서 투사체 속성 가져오기
        float damage = customDamage ?? (StatHandler.Instance != null ? StatHandler.Instance.AttackPower : 10f);
        ProjectileOptions options = customOptions ?? GetProjectileOptionsFromStatHandler();

        int projectileCount = StatHandler.Instance != null ? StatHandler.Instance.ProjectileCount : 1;

        if (projectileCount == 1)
        {
            SpawnProjectile(position, direction, damage, options);
            return;
        }

        // 투사체가 2개 이상이면 MultiAngle에 따라 퍼져서 발사
        float totalAngle = options.MultiAngle;
        if (totalAngle <= 0)
        {
            totalAngle = 5f;
        }

        float angleStep = totalAngle / (projectileCount - 1);

        float startAngle = -totalAngle / 2;

        for (int i = 0; i < projectileCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector2 shotDirection = RotateVector(direction, currentAngle);
            SpawnProjectile(position, shotDirection, damage, options);
        }
    }

    /// <summary>
    /// StatHandler에서 투사체 옵션 참조
    /// </summary>
    private ProjectileOptions GetProjectileOptionsFromStatHandler()
    {
        ProjectileOptions options = new ProjectileOptions();

        if (StatHandler.Instance != null)
        {
            options.Speed = StatHandler.Instance.ProjectileSpeed > 0 ?
                           StatHandler.Instance.ProjectileSpeed : 10f;
            options.Lifetime = StatHandler.Instance.ProjectileRange > 0 ?
                              StatHandler.Instance.ProjectileRange : 5f;
            options.Size = StatHandler.Instance.ProjectileSize;
            options.PenetrationCount = StatHandler.Instance.PenetrationCount;
            options.ReflectionCount = StatHandler.Instance.ReflectionCount;
            options.SpreadAngle = StatHandler.Instance.SpreadAngle;
            options.MultiAngle = StatHandler.Instance.MultiAngle;
        }

        return options;
    }

    /// <summary>
    /// 투사체 생성
    /// </summary>
    private void SpawnProjectile(Vector2 position, Vector2 direction, float damage, ProjectileOptions options)
    {
        // 옵션이 없으면 기본값 생성
        if (options == null)
        {
            options = new ProjectileOptions();
        }

        // 탄퍼짐(SpreadAngle) 계산
        if (options.SpreadAngle > 0)
        {
            float randomSpread = Random.Range(-options.SpreadAngle, options.SpreadAngle);
            direction = RotateVector(direction, randomSpread);
        }

        // 풀에서 투사체 가져오기
        Projectile projectile = GetProjectileFromPool();
        if (projectile == null)
        {
            return;
        }

        // 투사체 위치 및 회전 설정
        projectile.transform.position = position;
        projectile.gameObject.SetActive(true);

        // 활성 목록에 추가
        activeProjectiles.Add(projectile);

        // 투사체 초기화
        projectile.Initialize(
            damage,
            direction,
            options.Speed,
            options.Lifetime,
            options.Size,
            options.PenetrationCount,
            options.ReflectionCount
        );
    }

    /// <summary>
    /// 벡터를 특정 각도만큼 회전
    /// </summary>
    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }
}

/// <summary>
/// 투사체 옵션을 담는 클래스
/// </summary>
public class ProjectileOptions
{
    public float Speed = 10f;
    public float Lifetime = 5f;
    public float Size = 1f;
    public int PenetrationCount = 0;
    public int ReflectionCount = 0;
    public float SpreadAngle = 0f;
    public float MultiAngle = 0f;

    public ProjectileOptions() { }

    public ProjectileOptions(float speed, float lifetime, float size, int penetrationCount, int reflectionCount, float spreadAngle, float multiAngle)
    {
        Speed = speed;
        Lifetime = lifetime;
        Size = size;
        PenetrationCount = penetrationCount;
        ReflectionCount = reflectionCount;
        SpreadAngle = spreadAngle;
        MultiAngle = multiAngle;
    }
}