using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour, IDamageable
{
    [Header("Monster Stats")]
    [SerializeField] int hp;
    [SerializeField] int attackPower;
    [SerializeField] float attackSpeed = 1.5f;
    public GameObject dropItem;

    public bool isDie { get { return Hp <= 0; } }

    public int Hp 
    { 
        get => hp;
        set
        {
            hp = value;
            if (hp <= 0)// set을 호출해주는 시점에 체킹.
            {
                ChangeState(MonsterState.Dead); // 체력이 0 이하가 되면 즉시 Dead로 변경
                StartCoroutine(Dead());
            }
        }
    }
    public int AttackPower { get => attackPower; set => attackPower = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }

    [Header("Monster AI")]
    public MonsterState monsterState = MonsterState.Idle;
    public enum MonsterState
    {
        Idle,
        Move,// 이동시(일반몬스터 : 무조건 추적형식 + 거리되면 공격, 엘리트+보스몬스터 : 특정 거리 내의 이동할 위치를 찍고 만약 사이에 벽에 없다면 이동 실행 => 이후 공격, 벽이 있다면 재귀로 다시 실행) 
        Attack,// 근접 + 원거리
        Skill,
        Dead
    }
    public float detectRange;// 플레이어를 감지하는 범위, 거리 밖으로 나가면 target을 null로
    protected float distanceToWall;// 벽까지의 거리
    [SerializeField] protected float moveSpeed;// 이동 속도
    public PlayerController target;// 플레이어 == 타겟.
    public Animator anim;
    public Rigidbody2D rb;
    private Dictionary<MonsterState, Func<IEnumerator>> stateHandlers;// FSM 패턴의 코루틴을 실행하기 위한 딕셔너리.

    [Header("Monster etc")]
    public static Action<GameObject> OnMonsterDied;
    [SerializeField] protected MonsterData monsterData;
    SpriteRenderer spriteRenderer;
    Color originalColor;
    bool isBlinking = false;// 피격시 깜빡임
    [SerializeField] protected bool isLookRight = false;// 오리지널 스프라이트의 바라보는 방향값.

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
            TakeDamage(2);
    }
    private void OnEnable()
    {
        monsterState = MonsterState.Idle;// 상태 초기화.
    }
    private void Awake()
    {
        if (monsterData != null)
        {
            Hp = monsterData.maxHp;
            attackPower = monsterData.attackPower;
            attackSpeed = monsterData.attackSpeed;
            detectRange = monsterData.detectRange;
            moveSpeed = monsterData.moveSpeed;
        }   // 데이터 초기화.
    }
    protected virtual void Start()
    {
        if (dropItem == null)
            dropItem = Resources.Load<GameObject>("Drop/Chest");

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        distanceToWall = GetComponent<CircleCollider2D>().radius * 1.4f; // 벽까지 다가갈 수 있는 최소 거리는 몬스터의 콜라이더보다 조금 더 길게.
        stateHandlers = new Dictionary<MonsterState, Func<IEnumerator>>()
        {
            {MonsterState.Idle, Idle},
            {MonsterState.Move, Move},
            {MonsterState.Attack, Attack},
            {MonsterState.Skill,  Skill},
            {MonsterState.Dead, Dead}
        };

   

        StartCoroutine(StateMachine());
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();// 몬스터가 비활성화 되면 모든 코루틴을 멈춤.
        OnMonsterDied?.Invoke(gameObject);// 몬스터가 비활성화 되면(죽으면) 해당 맵에 이벤트 함수를 통해 알림.

        if (isDie) //씬 전환일때는 생성하지않도록
            DropItem();
    }

    protected IEnumerator StateMachine()
    {
        while (true)
        {
            if (stateHandlers.TryGetValue(monsterState, out var stateRoutine))
            {
                yield return StartCoroutine(stateRoutine()); // 상태 실행
            }
            yield return null;
        }
    }
    protected void ChangeState(MonsterState newState)
    {
        if (monsterState == newState) return;// 현재 상태와 새로운 상태가 같다면 리턴 (불필요한 상태 전환 방지)

        monsterState = newState;
    }
    protected abstract IEnumerator Idle();
    protected abstract IEnumerator Move();
    protected abstract IEnumerator Attack();
    protected virtual IEnumerator Skill() { yield break; }
    protected abstract IEnumerator Dead();


    public virtual void TakeDamage(int damage)// 데미지를 받는 함수
    {
        if(isDie) return;
        Hp -= damage;
        if(!isBlinking)
        {
            StartCoroutine(BlinkEffect());
        }
    }
    protected abstract void MonsterDead();// 몬스터가 죽었을 때 실행되는 함수
    public void SetTargetNull()// 플레이어가 다른 방으로 이동시 호출.
    {
        target = null;
    }
    private IEnumerator BlinkEffect()// 피격시 깜빡임 효과
    {
        isBlinking = true;
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f, 1f);// 피격시 연한 빨강색 2회 점멸.
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
        isBlinking = false;
    }
    public void DropItem()
    {
        Instantiate(dropItem, transform.position, Quaternion.identity);
    }

    /*
     * 1. Reaper, 근거리1개 + 원거리1개
일반 패턴 : 근거리 사거리내에 접근시 근거리 공격,

스킬 패턴 : target이 입력되고 처음 move state에 들어온 순간부터 쿨타임 카운트 시작. 
스킬 1 : 플레이어가 스킬 사거리(별도의 변수) 내에 접근하면 8초마다 플레이어의 위치로 매우 빠르게 대시.
스킬 2 : 매 14초 마다 플레이어의 근처
3곳에 랜덤하게 스킬 소환.


2. Dark Mage 원거리 2개
스킬 패턴 : target이 입력되고 처음 move state에 들어온 순간부터 쿨타임 카운트 시작.
매 4초마다, 2초동안 플레이어와 일정 거리를 두고, 거리를 둔 이후 스킬 1, 2 둘중 하나를 랜덤으로 사용.
스킬 1 : target이 있던 방향으로 벽에 닿으면 파괴되는 투사체 발사.
스킬 2 : 원형으로 투사체 12개 발사.

3. Agias
이동 패턴 : 매 10초마다 1.5초간 이동
스킬 패턴 : 나머지는 7초에 한번씩 스킬 2 또는 3을 사용(리퍼와 같은 방식으로 스킬 시전 구성) >> 스킬 1,2,3 순서대로 우선 사용.
입장시 : 입장 후 3초 이후부터 동작.
스킬 1(플레이어 근처 -3~3 랜덤 위치 별모양 검정 마법진 1개) : 
매 20초마다 유도하는 작은 눈알 몬스터 각각 3마리 소환. 부딫히면 터짐 (매우 빠른 속도)

스킬 2(빨간색 원이 차고 다차면 발사) : 맵에 5줄기 직선으로 광선이 뻗고 이후 3초간 회전
스킬 3(보라색 원이 차고 다차면 발사) : 맵에 무수히 많은 발사체를 원형으로 분사

    */// 엘리트 몬스터 패턴
}
