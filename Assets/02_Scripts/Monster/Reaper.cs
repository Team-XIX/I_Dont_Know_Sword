using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Reaper : MonsterBase
{
    [SerializeField] private float attackRange = 2.3f; // 공격 범위
    [SerializeField] private float dashRange = 10f; // 대쉬 범위
    [SerializeField] private float dashCoolTime = 8f; // 대쉬 쿨타임
    [SerializeField] private float deathHandCoolTime = 15f; // 데스핸드 쿨타임
    bool isNearWall = false;
    int wallLayerMask;

    [Header("Skill FX")]
    [SerializeField] private GameObject deathHandPrefab; // 데스핸드 프리팹
    [SerializeField] private ParticleSystem sandevistanParticle; // 산데비스탄 파티클
    [SerializeField] private Material sandeMaterial;
    [SerializeField] private Sprite sandeRight;
    [SerializeField] private Sprite sandeLeft;

    protected override void Start()
    {
        base.Start();
        if (monsterData != null)
            attackRange = monsterData.attackRange;
        wallLayerMask = LayerMask.GetMask("Wall");
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f); // 0.5초마다 경로 갱신
        InvokeRepeating(nameof(SkillUse), 0f, 3f); // 3초마다 스킬 사용 시도
        InvokeRepeating(nameof(IsNearWall), 0f, 0.1f); // 0.1초마다 벽 감지
    }

    private void Update()
    {
        dashCoolTime -= Time.deltaTime;
        deathHandCoolTime -= Time.deltaTime;
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
        if(target == null) return;
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
    private void MoveTo(Vector2 targetPos)
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
                sandeMaterial.mainTexture = sandeLeft.texture;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().flipX = target.transform.position.x > transform.position.x;
                sandeMaterial.mainTexture = sandeRight.texture;
            }

            // 공격 범위 내에 들어오면 Attack 상태로 변경
            if (Vector2.Distance(transform.position, target.transform.position) < attackRange)
            {
                ChangeState(MonsterState.Attack);
                yield break;
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
    protected override IEnumerator Attack()// 플레이어의 방향으로 공격 실행
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("Attack") == false)
        {
            anim.Play("Attack", 0, 0);
            yield return null;
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);// 상태정보 갱신
        }

        // 애니메이션이 끝날 때까지 기다림 (attack 애니메이션 도중에 move로 변경 방지)
        while (curAnimStateInfo.normalizedTime < 1.0f)
        {
            // 애니메이션 진행도를 계속 갱신
            curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            //진행 중에는 상태변경 대기.
            yield return null;
        }
        // while문이 끝나게 되면 상태 변경 조건 확인.
        if (Vector2.Distance(transform.position, target.transform.position) >= attackRange)// 타겟이 공격 범위 밖에 있으면 Move 상태로 변경.
        {
            ChangeState(MonsterState.Move);
        }
        else// 타겟이 아직 공격 범위 안에 있으면 공격 속도만큼 대기 후 다시 공격.
        {
            yield return new WaitForSeconds(curAnimStateInfo.length * AttackSpeed); // 살짝 대기 후 다시 공격
            ChangeState(MonsterState.Attack);
        }
    }
    protected override IEnumerator Skill()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (curAnimStateInfo.IsName("DeathHand") == false)
        {
            anim.Play("DeathHand", 0, 0);
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
        // 스킬 사용 후 상태 변경
        ChangeState(MonsterState.Move);
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
        Debug.Log("Monster Dead");
        gameObject.SetActive(false);
    }
    // 애니메이션 이벤트
    public void AnimEventAttack()// 애니메이션 이벤트를 통해 호출할 근접 공격 함수.
    {
        if (target == null) return;// 타겟이 없으면 리턴
        if (Vector2.Distance(transform.position, target.transform.position) < attackRange * 1.5f)// 실제 공격 애니메이션 시점에서 공격범위 1.5배내를 벗어나지 않았다면 데미지 연산.
            Debug.Log("Attack");//idamageable 인터페이스를 구현한 플레이어에게 데미지를 입히는 코드로 추후 변경.
    }

    // 몬스터 행동 패턴
    void SkillUse()
    {
        if(dashCoolTime <= 0)
        {
            Dash();
            return;
        }
        if(deathHandCoolTime <= 0)
        {
            DeathHand();
            return;
        }
    }// 사용가능한 스킬을 우선순위에 따라 사용 시도.
    void Dash()
    {
        if(target == null) return;
        if(monsterState == MonsterState.Move && Vector2.Distance(transform.position,target.transform.position) <= dashRange)// 이동 상태에서만 대쉬 가능.
        {
            dashCoolTime = 8f;
            monsterState = MonsterState.Idle;
            transform.DOMove(target.transform.position, 0.5f)
                .OnStart(() => sandevistanParticle.gameObject.SetActive(true))
                .OnComplete(() =>
                {
                    sandevistanParticle.gameObject.SetActive(false);
                    monsterState = MonsterState.Move;
                });
        }
    }
    void DeathHand()
    {
        if(target == null) return;
        if(monsterState == MonsterState.Move)// 이동 상태에서만 데스핸드 가능.
        {
            deathHandCoolTime = 15f;
            monsterState = MonsterState.Skill;
            //skillnum = 1; 이렇게 스킬NUM을 지정해서 Skill 상태에서 스킬의 종류를 구분해서 사용하는식으로 추후 추가 구현 가능.
        }
    }
    public void AnimEventSpawnHand()// 애니메이션 이벤트를 통해 호출할 데스핸드 생성 함수.
    {
        if(target == null) return;
        for(int i = 0; i < 3; i++)
        {
            GameObject deathHand = Instantiate(deathHandPrefab, target.transform.position + new Vector3(Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f),0), Quaternion.identity);
        }
    }
}
