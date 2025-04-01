using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

// 데이터베이스의 기본 추상 클래스
[System.Serializable]
public abstract class BaseData
{
    public int id;
    public string name;
    public string description;


    // 데이터 필드 설정 메서드
    //추상메서드
    public virtual void SetData(string[] headers, string[] values)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            switch (headers[i])
            {
                case "id":
                    id = int.Parse(values[i]);
                    break;
                case "name":
                    name = values[i];
                    break;
                case "description":
                    description = values[i];
                    break;
            }
        }
    }
}

