using UnityEngine;

[System.Serializable]
public class WeaponData : BaseData
{
    public int atk;
    public float atkSpeed;
    public int ammoCnt;

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
                case "atk":
                    atk = int.Parse(values[i]);
                    break;
                case "atkSpeed":
                    atkSpeed = float.Parse(values[i]);
                    break;
                case "ammoCnt":
                    ammoCnt = int.Parse(values[i]);
                    break;
            }
        }
    }
}
