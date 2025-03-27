using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonsterBase
{
    // Flood Fill �˰����� ����Ͽ� ���� ���� �̵��ϵ��� ����
    private Queue<Vector2> pathQueue = new Queue<Vector2>(); // Ž���� ��ġ ����
    private HashSet<Vector2> visited = new HashSet<Vector2>(); // �湮�� ��ġ
    [SerializeField] private float maxSearchDistance = 5f; // �ʹ� �ָ� Ž������ �ʵ��� ����
    [SerializeField] private float attackRange = 1f; // ���� ����
    int wallLayerMask;

    protected override void Start()
    {
        base.Start();
        wallLayerMask = LayerMask.GetMask("Wall");
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f); // 0.5�ʸ��� ��� ����
    }

    private void UpdatePath()
    {
        if (target == null || Vector2.Distance(transform.position, target.transform.position) > detectRange)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        // �÷��̾�� ���� ���̿� ���� �ִ��� Ȯ��
        if (IsWallBetweenPlayerAndMonster())
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        FindPath();
    }
    private bool IsWallBetweenPlayerAndMonster()
    {
        Vector2 monsterPos = transform.position;
        Vector2 targetPos = target.transform.position;

        // �÷��̾�� ���� ���̿� ���� �ִٸ� true ����
        return Physics2D.Linecast(monsterPos, targetPos, wallLayerMask);
    }

    private void FindPath()
    {
        Debug.Log("FindPath");
        pathQueue.Clear();
        visited.Clear();

        Vector2 start = transform.position;
        Vector2 targetPos = target.transform.position;

        pathQueue.Enqueue(start);
        visited.Add(start);

        int searchCount = 0; // Ž���� ��� �� ����
        bool pathFound = false; // ��θ� ã�Ҵ��� Ȯ��

        while (pathQueue.Count > 0)
        {
            Vector2 current = pathQueue.Dequeue();
            searchCount++;

            if (searchCount > maxSearchDistance) break; // �ʹ� ���� �� Ž�� ����

            List<Vector2> nextPositions = GetNeighborPositions(current);
            foreach (Vector2 next in nextPositions)
            {
                if (!visited.Contains(next) && !Physics2D.OverlapCircle(next, 0.5f, wallLayerMask))
                {
                    pathQueue.Enqueue(next);
                    visited.Add(next);

                    if (Vector2.Distance(next, targetPos) < 1.5f && !Physics2D.Linecast(current, next, wallLayerMask))
                    {
                        // ��� ã����
                        pathFound = true;
                        MoveTo(next);
                        return;
                    }
                }
            }
        }

        if (!pathFound)
        {
            // ��θ� ã�� ���ϸ� Idle ���·� ����
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
    private void MoveTo(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        float moveDistance = moveSpeed * Time.deltaTime;
        Vector2 nextPosition = (Vector2)transform.position + direction * moveDistance;

        // ���� �浹 üũ (���� ��ġ�� ������ Ȯ��)
        if (!Physics2D.OverlapCircle(nextPosition, 0.2f, wallLayerMask))
        {
            transform.position = nextPosition; // ���� ������ �̵�
        }
        else
        {
            // ���� �������� ��� �ٽ� Ž��
            FindPath();
        }
    }

    protected override IEnumerator Idle()
    {
        while (monsterState == MonsterState.Idle)
        {
            if (target != null && Vector2.Distance(transform.position, target.transform.position) < detectRange)
            {
                // ���� ������ �ٽ� Move ���·� ��ȯ
                if (!IsWallBetweenPlayerAndMonster())
                {
                    ChangeState(MonsterState.Move);
                }
            }
            yield return null;
        }
    }

    protected override IEnumerator Move()// �÷��̾� ���� + �Ÿ��� �Ǹ� Attack ���·� ����
    {
        while (monsterState == MonsterState.Move)
        {
            if(target == null)// �÷��̾ �Ÿ� ������ ����� target�� null�� ����.
            {
                ChangeState(MonsterState.Idle);
                yield break;
            }
            if(Vector2.Distance(transform.position, target.transform.position) < attackRange)
            {
                ChangeState(MonsterState.Attack);
                yield break;
            }
            // �� ���ǿ� �ش����� ������ Target�� ���� �̵�
            MoveTo(target.transform.position);
            yield return null;
        }
    }

    protected override IEnumerator Attack()// �÷��̾��� �������� ���� ����
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
