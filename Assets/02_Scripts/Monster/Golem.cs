using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonsterBase
{
    
    protected override IEnumerator Idle()
    {
        while (monsterState == MonsterState.Idle)
        {
            yield return null;
        }
    }

    protected override IEnumerator Move()
    {
        while (monsterState == MonsterState.Move)
        {
            yield return null;
        }
    }

    protected override IEnumerator Attack()
    {
        while (monsterState == MonsterState.Attack)
        {
            yield return null;
        }
    }

    protected override IEnumerator Dead()
    {
        while (monsterState == MonsterState.Dead)
        {
            yield return null;
        }
    }
}
