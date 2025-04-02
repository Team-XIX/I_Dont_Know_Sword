using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMage : MonsterBase
{
    [Header("Monster Value")]
    [SerializeField] private float projectileSpeed = 7f; // 투사체 속도
    [SerializeField] private int samplePoints = 8; // 샘플링할 점의 개수 (몬스터의 콜라이더가 벽 콜라이더와 겹침을 확인하기 위한 샘플 포인트)
    CircleCollider2D circleCollider;
    bool isNearWall = false;
    int wallLayerMask;

    [Header("Skill FX")]
    [SerializeField] private List<GameObject> projectilePool; // 투사체 풀
    [SerializeField] private GameObject monsterProjectile; // 몬스터 투사체
    [SerializeField] private Transform shootRight;// 오른쪽을 바라볼때 투사체 발사 시작위치.
    [SerializeField] private Transform shootLeft;// 왼쪽을 바라볼때 투사체 발사 시작위치.

    protected override void Start()
    {
        base.Start();
        circleCollider = GetComponent<CircleCollider2D>();
        wallLayerMask = LayerMask.GetMask("Wall");
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f); // 0.5초마다 경로 갱신
        InvokeRepeating(nameof(IsNearWall), 0f, 0.1f); // 0.1초마다 벽 감지
        InvokeRepeating(nameof(SetMove), 0f, 3f); // 4초마다 이동시작 (이동 시작후 2초후 스킬 사용)
    }

    // 몬스터 이동 패턴
    private void UpdatePath()
    {
        if (target == null || Vector2.Distance(transform.position, target.transform.position) > detectRange)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        // 벽이 너무 가까우면 이동 중지
        if (isNearWall)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        // 플레이어를 향해 이동
        MoveTo(target.transform.position);
    }
    private void IsNearWall()
    {
        if (target == null) return;
        Vector2 monsterPos = transform.position;
        Vector2 targetPos = target.transform.position;

        // 몬스터와 플레이어 사이에 벽이 있는지 확인
        RaycastHit2D hit = Physics2D.Linecast(monsterPos, targetPos, wallLayerMask);
        if (hit.collider != null) // 벽이 감지되었을 경우
        {
            Debug.Log("Wall");
            isNearWall = Vector2.Distance(monsterPos, hit.point) <= distanceToWall;
        }
        else
        {
            Debug.Log("No Wall");
            isNearWall = false; // 벽이 없거나, 벽과 충분한 거리가 있다면 false
        }
    }
    private void MoveTo(Vector2 targetPos)// direction에 단위벡터를 받고 deltatime과 이동 속도를 통해 프레임당 이동할 거리를 받아 transform의 위치를 매 프레임마다 이동 시켜주는 메서드.
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        float moveDistance = moveSpeed * Time.deltaTime;
        Vector2 nextPosition = (Vector2)transform.position + direction * moveDistance;

        // 벽과의 거리 계산
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, direction, moveDistance + distanceToWall, wallLayerMask);

        if (wallHit.collider != null)
        {
            // 벽과의 거리가 일정 이하이면 이동하지 않음
            if (Vector2.Distance(transform.position, wallHit.point) <= distanceToWall)
            {
                return; // 이동하지 않음
            }
        }

        // 벽이 없거나 충분한 거리가 있으면 이동
        transform.position = nextPosition;
    }

    // 몬스터 행동 패턴
    void SetMove()// 일정 시간마다 2초간 움직임.
    {
        if (!this.gameObject.activeSelf) return;
        // 벽에 끼인 상태면 강제로 플레이어를 향해 이동.
        if (IsStuckInWall())
            StartCoroutine(ForceMoveFromWall());

        monsterState = MonsterState.Move;
        StartCoroutine(MoveAttack());
    }
    bool IsStuckInWall()// 몬스터가 벽에 끼였는지 확인. (비상 탈출용)
    {
        if (circleCollider == null) return false;

        Vector2 center = circleCollider.bounds.center;
        float radius = circleCollider.radius * transform.localScale.x; // 실제 반지름 고려
        int wallCount = 0;

        // 원 주변을 샘플링해서 Wall과 겹치는 부분 체크
        for (int i = 0; i < samplePoints; i++)
        {
            float angle = (i / (float)samplePoints) * 2 * Mathf.PI;
            Vector2 samplePoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            if (Physics2D.OverlapPoint(samplePoint, wallLayerMask))
            {
                wallCount++;
            }
        }

        // 겹친 비율이 설정값을 넘으면 true 반환
        return (wallCount / (float)samplePoints) >= 0.3f;// 30% 이상이면 벽에 끼인 것으로 판단.
    }
    IEnumerator ForceMoveFromWall()// 1.5초 동안 강제로 플레이어의 위치로 이동.
    {
        if (target == null) yield break;

        float time = 0f;
        Vector2 targetPos = target.transform.position;
        Vector2 startPos = transform.position;
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, targetPos, time);
            yield return null;
        }
    }
    IEnumerator MoveAttack()
    {
        yield return new WaitForSeconds(2f);
        monsterState = MonsterState.Skill;
    }

    // 몬스터 상태 머신
    protected override IEnumerator Idle()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("Idle") == false)
        {
            anim.Play("Idle", 0, 0);
            yield return null;
        }
        anim.Play("Idle", 0, 0);

        while (monsterState == MonsterState.Idle)
        {
            if (target != null && Vector2.Distance(transform.position, target.transform.position) < detectRange)
            {
                // 벽이 없으면 다시 Move 상태로 전환
                if (!isNearWall)
                {
                    ChangeState(MonsterState.Move);
                }
            }
            yield return null;
        }
    }
    protected override IEnumerator Move()// 플레이어 추적 + 거리가 되면 Attack 상태로 변경
    {
        // 상태 변경전 한번더 체크.
        yield return null;
        if (isNearWall)
        {
            ChangeState(MonsterState.Idle);
            yield break;
        }

        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("Move") == false)
        {
            anim.Play("Move", 0, 0);
            yield return null;
        }

        while (monsterState == MonsterState.Move)
        {
            // 타겟 체킹
            if (target == null)
            {
                ChangeState(MonsterState.Idle);
                yield break;
            }
            // 플레이어의 위치에 따른 스프라이트 x flip 조정 (오리지널 스프라이트 기준 if else 처리)
            if (isLookRight)
            {
                this.GetComponent<SpriteRenderer>().flipX = target.transform.position.x < transform.position.x;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().flipX = target.transform.position.x > transform.position.x;
            }

            // 벽에 너무 가까워졌다면 멈추고 Idle 상태로 변경
            if (isNearWall)
            {
                ChangeState(MonsterState.Idle);
                yield break;
            }

            // 플레이어를 향해 이동
            MoveTo(target.transform.position);
            yield return null;
        }
    }
    protected override IEnumerator Attack()
    {
        throw new System.NotImplementedException();
    }
    protected override IEnumerator Skill()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        int random = Random.Range(0, 3);

        if (random == 0)// 1/3 확률로 플레이어의 위치로 투사체 연속 발사.
        {
            if (curAnimStateInfo.IsName("BloodShot") == false)
            {
                anim.Play("BloodShot", 0, 0);
                yield return null;
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);// 상태정보 갱신
            }
        }
        else// 2/3 확률로 원형으로 투사체 24개 발사.
        {
            if (curAnimStateInfo.IsName("BloodExplode") == false)
            {
                anim.Play("BloodExplode", 0, 0);
                yield return null;
                curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);// 상태정보 갱신
            }
        }

        while (curAnimStateInfo.normalizedTime < 1.0f)
        {
            // 애니메이션 진행도를 계속 갱신
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            //진행 중에는 상태변경 대기.
            yield return null;
        }
        // 스킬 사용 후 상태 변경
        ChangeState(MonsterState.Idle);
        yield return null;
    }
    protected override IEnumerator Dead()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("Dead") == false)
        {
            anim.Play("Dead", 0, 0);
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);// 상태정보 갱신
        }

        while (curAnimStateInfo.normalizedTime < 1.0f)
        {
            // 애니메이션 진행도를 계속 갱신
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            //진행 중에는 상태변경 대기.
            yield return null;
        }

        MonsterDead();// 몬스터 사망 처리
        yield return null;
    }
    protected override void MonsterDead()
    {
        gameObject.SetActive(false);
    }

    // 애니메이션 이벤트
    public void AnimEventBloodShot()// 애니메이션 이벤트를 통해 투사체 발사
    {
        if (target == null) return;
        foreach (var projectile in projectilePool)
        {
            if (projectile.activeSelf == false)
            {
                projectile.transform.position = target.transform.position.x < transform.position.x ? shootLeft.position : shootRight.position;
                projectile.SetActive(true);
                projectile.GetComponent<Rigidbody2D>().velocity = (target.transform.position - transform.position).normalized * projectileSpeed;
                break;
            }
        }
        // 만약 풀에 사용 가능한 투사체가 없다면 새로 생성
        GameObject newProjectile = Instantiate(monsterProjectile, target.transform.position.x < transform.position.x ? shootLeft.position : shootRight.position, Quaternion.identity);
        projectilePool.Add(newProjectile);
        newProjectile.GetComponent<Rigidbody2D>().velocity = (target.transform.position - transform.position).normalized * projectileSpeed;
    }
    public void AnimEventBloodExplode()// 애니메이션 이벤트를 통해 호출할 전범위 투사체 발사 한번에 24개에
    {
        if (target == null) return;// 타겟이 없으면 리턴
        
        int count = 0;
        // 풀에 사용 가능한 투사체가 있는지 확인 후 발사
        foreach (var projectile in projectilePool)
        {
            if (!projectile.activeSelf)
            {
                FireProjectile(projectile, count);
                count++;
                if (count >= 24) break;
            }
        }

        // 부족한 투사체 생성
        while (count < 24)
        {
            GameObject newProjectile = Instantiate(monsterProjectile, transform.position, Quaternion.identity);
            projectilePool.Add(newProjectile);
            FireProjectile(newProjectile, count);
            count++;
        }
    }
    void FireProjectile(GameObject projectile, int index)// 투사체 발사(코드 재사용성을 위한 분리)
    {
        projectile.transform.position = transform.position;
        projectile.SetActive(true);

        float angle = 360f / 24f * index * Mathf.Deg2Rad; // 각도를 360도로 나누어 계산
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * projectileSpeed;
    }
}
