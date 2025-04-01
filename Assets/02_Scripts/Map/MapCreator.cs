using System;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [Header("맵 안에서 쓰일 방 리스트 : 일반 방")]
    public List<Room> roomPrefabs;

    [Header("맵 안에서 쓰일 방 리스트 : 엘리트 방")]
    public List<Room> roomElitePrefabs;

    [Header("맵 생성 후 맵으로 구성된 방들")]
    public List<Room> roomMap;

    [Header("맵 안의 한변에 있을 방 개수 (정사각형)")]
    public int mapLength;

    [Header("방 사이 간격")]
    [SerializeField]
    private int roomInterval;

    [Header("현재 있는 방의 번호")]
    [SerializeField]
    private int currentRoomNum;

    //플레이어 이동시키는 Action
    public static Action<Direction> teleportAction;

    public PlayerController playerController;

    private void Start()
    {
        mapLength = 5; 
        roomInterval = 30;

        //초기 시작
        currentRoomNum = 0;
        teleportAction += Teleport;

        CreateMap();

        playerController.transform.position = roomMap[0].spawnPoint[1].position;
        ShowPlayerPosition();
    }

    //맵 생성
    void CreateMap()
    {
        for (int i = 0; i < mapLength; i++)
        {
            for (int j = 0; j < mapLength; j++)
            {
                Room room;
                if (i + j == mapLength - 1)
                {
                    room = roomElitePrefabs[UnityEngine.Random.Range(0, roomElitePrefabs.Count)];
                }
                else
                {
                    room = roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Count)];
                }
                var now = Instantiate(room, this.transform.position + new Vector3((room.roomLength + roomInterval) * j,
                    (room.roomLength + roomInterval) * i, 0), Quaternion.identity, this.transform);
                CheckEnterance(now, i, j);
                roomMap.Add(now);
            }
        }
    }

    //입구 관리
    void CheckEnterance(Room room, int height, int width)
    {
        //위쪽 입구 봉쇄
        if (height == mapLength - 1)
        {
            room.ControlEnterance(Direction.Up);
        }
        //밑쪽 입구 봉쇄
        else if (height == 0)
        {
            room.ControlEnterance(Direction.Down);
        }

        //왼쪽 입구 봉쇄
        if (width == 0)
        {
            room.ControlEnterance(Direction.Left);
        }
        //오른쪽 입구 봉쇄
        else if (width == mapLength - 1)
        {
            room.ControlEnterance(Direction.Right);
        }
    }

    //방간의 이동 구현
    public void Teleport(Direction dir)
    {
        if (!roomMap[currentRoomNum].isCleared)
        {
            return;
        }
        ErasePlayerPosition();
        switch (dir)
        {
            case Direction.Up:
                currentRoomNum += mapLength;
                playerController.transform.position = roomMap[currentRoomNum].spawnPoint[1].position;
                break;
            case Direction.Down:
                currentRoomNum -= mapLength;
                playerController.transform.position = roomMap[currentRoomNum].spawnPoint[0].position;
                break;
            case Direction.Left:
                currentRoomNum -= 1;
                playerController.transform.position = roomMap[currentRoomNum].spawnPoint[3].position;
                break;
            case Direction.Right:
                currentRoomNum += 1;
                playerController.transform.position = roomMap[currentRoomNum].spawnPoint[2].position;
                break;
            default: break;
        }
        ShowPlayerPosition();
    }

    //현재 방에 있는 모든 몬스터 죽이기
    public void KillMostersInCurrentRoom()
    {
        roomMap[currentRoomNum].KillAllMonstersInRoom();
    }

    //현재 방에 있는 입구 개방 
    public void OpenEnteranceInCurrentRoom()
    {
        roomMap[currentRoomNum].OpenEnterance();
    }

    //현재 방에 있는 입구 봉쇄
    public void BlockEnteranceInCurrentRoom()
    {
        roomMap[currentRoomNum].CloseEnterance();
    }

    //현재 맵에 있는 모든 몬스터 죽이기
    public void KillMosters()
    {
        foreach (Room room in roomMap)
        {
            room.KillAllMonstersInRoom();
        }
    }

    //모든 방의 입구 개방
    public void OpenAllRooms()
    {
        foreach (Room room in roomMap)
        {
            room.RoomClear();
        }
    }

    //모든 방의 입구 봉쇄
    public void BlockAllRooms()
    {
        foreach (Room room in roomMap)
        {
            room.CloseEnterance();
            room.isCleared = false;
        }
    }

    //즉시 보스방으로 텔레포트
    public void TeleportToBossRoom()
    {
        currentRoomNum = roomMap.Count - 1;
        playerController.transform.position = roomMap[currentRoomNum].spawnPoint[1].position;
    }

    //미니맵에 현재 위치 표시
    public void ShowPlayerPosition()
    {
        roomMap[currentRoomNum].PaintPath();
        roomMap[currentRoomNum].PaintCurrentPos();
    }

    //미니맵에 현재 위치 지우기
    public void ErasePlayerPosition()
    {
        roomMap[currentRoomNum].EraseCurrentPos();
    }
}
