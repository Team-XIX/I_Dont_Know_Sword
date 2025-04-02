using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public enum EItemType
{
    Health = 1,
    Speed,
    Atk,
    AtkSpeed
}

// 아이템 데이터 클래스
[System.Serializable]
public class ItemData : BaseData
{
    public int value;
    public bool isPermanent;
    public float time;
    public EItemType type;
    public string spritePath;

    public Sprite GetSprite()
    {
        //아이템 스프라이트는 리소스폴더안에 스프라이트의 이름을 id번호로
        //return Resources.Load<Sprite>($"ItemSprite/{id}");
        return Resources.Load<Sprite>(spritePath);
    }


    public override void SetData(string[] headers, string[] values)
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
                case "value":
                    value = int.Parse(values[i]);
                    break;
                case "isPermanent":
                    isPermanent = bool.Parse(values[i]);
                    break;
                case "time":
                    time = float.Parse(values[i]);
                    break;
                case "type":
                    type = (EItemType)int.Parse(values[i]);
                    break;
                case "spritePath":
                    spritePath = values[i];
                    break;
            }
        }
    }
}



