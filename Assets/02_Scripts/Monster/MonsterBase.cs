using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("Monster Stats")]
    [SerializeField] int hp;
    [SerializeField] int attackPower;

    public int Hp 
    { 
        get => hp;
        set
        {
            hp = value;
            if (hp <= 0)
            {
                ChangeState(MonsterState.Dead); // ü���� 0 ���ϰ� �Ǹ� ��� Dead�� ����
            }
        }
    }
    public int AttackPower { get => attackPower; set => attackPower = value; }

    [Header("Monster AI")]
    public MonsterState monsterState = MonsterState.Idle;
    public enum MonsterState
    {
        Idle,
        Move,// �̵���(�Ϲݸ��� : ������ �������� + �Ÿ��Ǹ� ����, ����Ʈ+�������� : Ư�� �Ÿ� ���� �̵��� ��ġ�� ��� ���� ���̿� ���� ���ٸ� �̵� ���� => ���� ����, ���� �ִٸ� ��ͷ� �ٽ� ����) 
        Attack,// ���� + ���Ÿ�
        Dead
    }
    public float detectRange;// �÷��̾ �����ϴ� ����, �Ÿ� ������ ������ target�� null��
    [SerializeField] protected float moveSpeed;// �̵� �ӵ�
    public PlayerController target;// �÷��̾� == Ÿ��.
    public Animator anim;
    public Rigidbody2D rb;
    private Dictionary<MonsterState, Func<IEnumerator>> stateHandlers;// FSM ������ �ڷ�ƾ�� �����ϱ� ���� ��ųʸ�.

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateHandlers = new Dictionary<MonsterState, Func<IEnumerator>>()
        {
            {MonsterState.Idle, Idle},
            {MonsterState.Move, Move},
            {MonsterState.Attack, Attack},
            {MonsterState.Dead, Dead}
        };
        StartCoroutine(StateMachine());
    }

    protected IEnumerator StateMachine()
    {
        while (true)
        {
            if (stateHandlers.TryGetValue(monsterState, out var stateRoutine))
            {
                yield return StartCoroutine(stateRoutine()); // ���� ����
            }
            yield return null;
        }
    }
    protected void ChangeState(MonsterState newState)
    {
        if (monsterState == newState) return;// ���� ���¿� ���ο� ���°� ���ٸ� ���� (���ʿ��� ���� ��ȯ ����)

        monsterState = newState;
    }
    protected abstract IEnumerator Idle();
    protected abstract IEnumerator Move();
    protected abstract IEnumerator Attack();
    protected abstract IEnumerator Dead();


    protected virtual void TakeDamage(int damage)// �������� �޴� �Լ�
    {

    }
    protected virtual void MeleeAttack()// ���� ���� (���� ���͸� ������ ��� ���Ͱ� ������ �ִ� ����)
    {

    }
    protected virtual void RangePattern()// ���Ÿ� ���� (����Ʈ ���Ϳ� ���� ���Ͱ� ������ �ִ� ����)
    {
        // �� ���� ���� ���� Ŭ�󽺿��� ������ �������� ���Ÿ� ���� ������ �������� ����
    }

    //
    public void SetTargetNull()// �÷��̾ �ٸ� ������ �̵��� ȣ��.
    {
        target = null;
    }
}
