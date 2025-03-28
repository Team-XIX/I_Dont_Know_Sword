using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Monster Status")]
    public int maxHp;
    public int attackPower;
    public float attackSpeed;
    public float attackRange;
    [Header("Monster AI & etc")]
    public float detectRange;
    public float moveSpeed;
}
