using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonster : MonsterBase
{
    [SerializeField] private float maxSearchDistance = 5f; // 너무 멀리 탐색하지 않도록 제한
    [SerializeField] private float attackRange = 1f; // 공격 범위
    int wallLayerMask;

    protected override void Start()
    {
        base.Start();
        wallLayerMask = LayerMask.GetMask("Wall");
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f); // 0.5초마다 경로 갱신
    }

    private void UpdatePath()
    {
        if (target == null || Vector2.Distance(transform.position, target.transform.position) > detectRange)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        // 벽이 너무 가까우면 이동 중지
        if (IsNearWall())
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        // 플레이어를 향해 이동
        MoveTo(target.transform.position);
    }
    private bool IsNearWall()
    {
        Vector2 monsterPos = transform.position;
        Vector2 targetPos = target.transform.position;

        // 몬스터와 플레이어 사이에 벽이 있는지 확인
        if (Physics2D.Linecast(monsterPos, targetPos, wallLayerMask))
        {
            // 벽과의 거리 계산
            RaycastHit2D wallHit = Physics2D.Raycast(monsterPos, (targetPos - monsterPos).normalized, detectRange, wallLayerMask);
            if (wallHit.collider != null && Vector2.Distance(monsterPos, wallHit.point) <= distanceToWall)
            {
                return true; // 벽에 너무 가까워지면 true
            }
        }
        
        return false; // 벽이 없거나, 벽과 충분한 거리가 있다면 false
    }

    /*
    private void FindPath()
    {
        Debug.Log("FindPath");
        pathQueue.Clear();
        visited.Clear();

        Vector2 start = transform.position;
        Vector2 targetPos = target.transform.position;

        pathQueue.Enqueue(start);
        visited.Add(start);

        int searchCount = 0; // 탐색한 노드 수 제한
        bool pathFound = false; // 경로를 찾았는지 확인

        while (pathQueue.Count > 0)
        {
            Vector2 current = pathQueue.Dequeue();
            searchCount++;

            if (searchCount > maxSearchDistance) break; // 너무 많은 곳 탐색 방지

            List<Vector2> nextPositions = GetNeighborPositions(current);
            foreach (Vector2 next in nextPositions)
            {
                if (!visited.Contains(next) && !Physics2D.OverlapCircle(next, 0.5f, wallLayerMask))
                {
                    pathQueue.Enqueue(next);
                    visited.Add(next);

                    if (Vector2.Distance(next, targetPos) < 1.5f && !Physics2D.Linecast(current, next, wallLayerMask))
                    {
                        // 경로 찾으면
                        pathFound = true;
                        MoveTo(next);
                        return;
                    }
                }
            }
        }

        if (!pathFound)
        {
            // 경로를 찾지 못하면 Idle 상태로 변경
            ChangeState(MonsterState.Idle);
        }
    }
    private List<Vector2> GetNeighborPositions(Vector2 current)
    {
        return new List<Vector2>
    {
        current + Vector2.up,
        current + Vector2.down,
        current + Vector2.left,
        current + Vector2.right
    };
    }
    */// flood fill 알고리즘 폐기.Raycasting 방식으로 변경.

    private void MoveTo(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        float moveDistance = moveSpeed * Time.deltaTime;
        Vector2 nextPosition = (Vector2)transform.position + direction * moveDistance;

        // 벽과의 거리 계산
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, direction, moveDistance + 0.6f, wallLayerMask);

        if (wallHit.collider != null)
        {
            // 벽과의 거리가 0.6 이하이면 이동하지 않음
            if (Vector2.Distance(transform.position, wallHit.point) <= distanceToWall)
            {
                return; // 이동하지 않음
            }
        }

        // 벽이 없거나 충분한 거리가 있으면 이동
        transform.position = nextPosition;
    }

    protected override IEnumerator Idle()
    {
        while (monsterState == MonsterState.Idle)
        {
            if (target != null && Vector2.Distance(transform.position, target.transform.position) < detectRange)
            {
                // 벽이 없으면 다시 Move 상태로 전환
                if (!IsNearWall())
                {
                    ChangeState(MonsterState.Move);
                }
            }
            yield return null;
        }
    }

    protected override IEnumerator Move()// 플레이어 추적 + 거리가 되면 Attack 상태로 변경
    {
        while (monsterState == MonsterState.Move)
        {
            if (target == null)
            {
                ChangeState(MonsterState.Idle);
                yield break;
            }

            if (Vector2.Distance(transform.position, target.transform.position) < attackRange)
            {
                ChangeState(MonsterState.Attack);
                yield break;
            }

            // 벽에 너무 가까워졌다면 멈추고 Idle 상태로 변경
            if (IsNearWall())
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
        while (monsterState == MonsterState.Attack)
        {
            Debug.Log("Attack");
            yield return null;
        }
    }

    protected override IEnumerator Dead()
    {
        while (monsterState == MonsterState.Dead)
        {
            Debug.Log("Dead");
            yield return null;
        }
    }
}
