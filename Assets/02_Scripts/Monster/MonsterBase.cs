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
    public GameObject[] dropItems;
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

    private bool isQuitting = false; //종료 체크 bool변수

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
        if (dropItems.Length ==0)
            dropItems = Resources.LoadAll<GameObject>("PrefabItem");

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

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();// 몬스터가 비활성화 되면 모든 코루틴을 멈춤.
        OnMonsterDied?.Invoke(gameObject);// 몬스터가 비활성화 되면(죽으면) 해당 맵에 이벤트 함수를 통해 알림.
        if (!isQuitting) //게임 종료일때는 랜덤 생성하지않도록
            RandomDrop();
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
        if(Hp <= 0) return;
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

    public void RandomDrop()
    {
        int randNum = UnityEngine.Random.Range(0, dropItems.Length);
        Instantiate(dropItems[randNum], transform.position, Quaternion.identity);
    }
}
