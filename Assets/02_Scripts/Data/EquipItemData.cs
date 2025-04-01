using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipItemData : BaseData
{
    public float value;
    public int type;
    public bool canStack;
    public int maxStackAmount;
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
                    value = float.Parse(values[i]);
                    break;
                case "type":
                    type = int.Parse(values[i]);
                    break;
                case "canStack":
                    canStack = bool.Parse(values[i]);
                    break;
                case "maxStackAmount":
                    maxStackAmount = int.Parse(values[i]);
                    break;
                case "spritePath":
                    spritePath = values[i];
                    break;

            }
        }
    }
}
