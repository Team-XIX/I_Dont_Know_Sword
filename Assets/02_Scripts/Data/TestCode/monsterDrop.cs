using System.Collections.Generic;
using UnityEngine;

public class monsterDrop : MonoBehaviour
{
    public GameObject[] dropItems;

    private void Start()
    {
        if(dropItems.Length == 0)
            dropItems = Resources.LoadAll<GameObject>("PrefabItem");
    }

    public void RandomDrop()
    {
        int randNum = Random.Range(0, dropItems.Length);
        Instantiate(dropItems[randNum], transform.position, Quaternion.identity);
        if (70 < randNum)
        {
            //Item
            Instantiate(dropItems[1], transform.position, Quaternion.identity);
        }
        else if (95 < randNum)
        {
            //EquipItem
            Instantiate(dropItems[0], transform.position, Quaternion.identity);
        }
        else if (100 < randNum)
        {
            //weapon
            Instantiate(dropItems[2], transform.position, Quaternion.identity);
        }
     
    }
}
