using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [Header("맵 안에서 쓰일 방 리스트")]
    public List<Room> roomPrefabs;

    [Header("맵의 가로/세로 길이")]
    public int mapWidth;
    public int mapHeight;

    [Header("방 사이 간격")]
    [SerializeField]
    private int roomInterval;

    private void Start()
    {
        //임시로 대입하여 작업
        mapWidth = 6; mapHeight = 6;
        roomInterval = 3;

        CreateMap();
    }

    void CreateMap()
    {
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                Room room = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                CheckEnterance(room, i, j);
                Instantiate(room, this.transform.position + new Vector3((room.roomLength + roomInterval) * j,
                    (room.roomLength + roomInterval) * i, 0), Quaternion.identity, this.transform);
                room.InitailEnterance();
            }
        }
    }
    
    void CheckEnterance(Room room, int height, int width)
    {
        //위쪽 출구 봉쇄
        if (height == mapHeight - 1)
        {
            room.ControlEnterance(Direction.Up);
        }
        //밑쪽 출구 봉쇄
        else if (height == 0)
        {
            room.ControlEnterance(Direction.Down);
        }

        //왼쪽 출구 봉쇄
        if (width == 0)
        {
            room.ControlEnterance(Direction.Left);
        }
        //오른쪽 출구 봉쇄
        else if (width == mapWidth - 1)
        {
            room.ControlEnterance(Direction.Right);
        }
    }

}
