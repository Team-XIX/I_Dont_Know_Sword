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

    public int Hp { get => hp; set => hp = value; }
    public int AttackPower { get => attackPower; set => attackPower = value; }

    [Header("Monster AI")]
    public MonsterState monsterState;
    public enum MonsterState
    {
        Idle,
        Move,// 이동시(일반몬스터 : 무조건 추적형식 + 거리되면 공격, 엘리트+보스몬스터 : 특정 거리 내의 이동할 위치를 찍고 만약 사이에 벽에 없다면 이동 실행 => 이후 공격, 벽이 있다면 재귀로 다시 실행) 
        Attack,// 근접 + 원거리
        Dead
    }
    public Animator anim;
    public TestPlayer target;
    private Dictionary<MonsterState, Func<IEnumerator>> stateHandlers;// FSM 패턴의 코루틴을 실행하기 위한 딕셔너리.

    private void Start()
    {
        anim = GetComponent<Animator>();
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
    protected abstract IEnumerator Dead();


    protected virtual void TakeDamage(int damage)// 데미지를 받는 함수
    {

    }
    protected virtual void MeleeAttack()// 근접 공격 (보스 몬스터를 제외한 모든 몬스터가 가지고 있는 패턴)
    {

    }
    protected virtual void RangePattern()// 원거리 공격 (엘리트 몬스터와 보스 몬스터가 가지고 있는 패턴)
    {
        // 각 몬스터 마다 개별 클라스에서 구현한 여러가지 원거리 공격 패턴을 랜덤으로 실행
    }
}
