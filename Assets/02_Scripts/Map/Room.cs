using System;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up, Down, Left, Right
}

public class Room : MonoBehaviour
{
    [Header("방의 한변의 길이")]
    public int roomLength = 30;

    [Header("방의 입구들 => 상 하 좌 우 순서")]
    public List<GameObject> roomEnterance;

    [Header("방의 입구들 봉쇄벽 => 상 하 좌 우 순서")]
    public List<GameObject> roomBlock;

    [Header("방의 스폰포인트")]
    public List<Transform> spawnPoint;

    [Header("방의 몬스터 리스트")]
    [SerializeField]
    public List<GameObject> monsterList; 

    [Header("방의 클리어 여부")]       //몬스터가 다 없어진 후에 true를 반환해주어야 함
    public bool isCleared = false;

    private void Start()
    {
        //몬스터가 죽었을때의 이벤트 구독 예정
        MonsterBase.OnMonsterDied += CheckClear;
    }

    //방 생성시 입구 관리
    public void ControlEnterance(Direction dir)
    {
        switch(dir)
        {
            case Direction.Up:
                roomEnterance[0].SetActive(false);
                roomBlock[0].SetActive(true);
                break;
            case Direction.Down:
                roomEnterance[1].SetActive(false);
                roomBlock[1].SetActive(true);
                break;
            case Direction.Left:
                roomEnterance[2].SetActive(false);
                roomBlock[2].SetActive(true);
                break;
            case Direction.Right:
                roomEnterance[3].SetActive(false);
                roomBlock[3].SetActive(true);
                break;
            default: break;
        }

        //조건을 달성하기 전 다른 방으로 이동 못하게 막기
        CloseEnterance();
    }

    //입구 초기화
    public void InitailEnterance()
    {
        foreach(GameObject enterance in roomEnterance)
        {
            enterance.SetActive(true);
        }
        foreach (GameObject block in roomBlock)
        {
            block.SetActive(false);
        }
    }

    //모든 입구 오픈
    public void OpenEnterance()
    {
        foreach (GameObject re in roomEnterance)
        {
            re.GetComponent<Collider2D>().isTrigger = true;
        }
    }

    //모든 입구 봉쇄
    public void CloseEnterance()
    {
        foreach (GameObject re in roomEnterance)
        {
            re.GetComponent<Collider2D>().isTrigger = false;
        }
    }

    //방 내 모든 몬스터 죽이기
    public void KillAllMonstersInRoom()
    {
        for(int i = monsterList.Count - 1; i >= 0; i--)
        {
            monsterList[i].SetActive(false);
        }
    }

    //클리어 여부 체크
    public void CheckClear(GameObject monster)
    {
        monsterList.Remove(monster);
        if(monsterList.Count == 0)
        {
            RoomClear();
            MonsterBase.OnMonsterDied -= CheckClear;
        }
    }

    //클리어 시 실행되는 함수
    public void RoomClear()
    {
        isCleared = true;
        OpenEnterance();
    }
}
