using UnityEngine;

[System.Serializable]
public class WeaponData : BaseData
{
    public float atk;
    public float moveSpeed;
    public float atkSpeed;
    public float spreadAngle;
    public float multiAngle;
    public int projectileCnt;
    public float projectileSize;
    public float projectileSpeed;
    public float projectileRange;
    public int reflectionCnt;
    public int penetrationCnt;
    public bool autoFire;

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
                    atk = float.Parse(values[i]);
                    break;
                case "moveSpeed":
                    moveSpeed = float.Parse(values[i]);
                    break;
                case "atkSpeed":
                    atkSpeed = float.Parse(values[i]);
                    break;
                case "spreadAngle":
                    spreadAngle = float.Parse(values[i]);
                    break;
                case "multiAngle":
                    multiAngle = float.Parse(values[i]);
                    break;
                case "projectileCnt":
                    projectileCnt = int.Parse(values[i]);
                    break;
                case "projectileSize":
                    projectileSize = float.Parse(values[i]);
                    break;
                case "projectileSpeed":
                    projectileSpeed = float.Parse(values[i]);
                    break;
                case "projectileRange":
                    projectileRange = float.Parse(values[i]);
                    break;
                case "reflectionCnt":
                    reflectionCnt = int.Parse(values[i]);
                    break;
                case "penetrationCnt":
                    penetrationCnt = int.Parse(values[i]);
                    break;           
                case "autoFire":
                    autoFire = bool.Parse(values[i]);
                    break;

            }
        }
    }
}
