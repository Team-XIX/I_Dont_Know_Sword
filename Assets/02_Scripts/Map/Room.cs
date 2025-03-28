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


}
