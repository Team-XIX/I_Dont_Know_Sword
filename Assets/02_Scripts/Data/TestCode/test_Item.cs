using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_Item : MonoBehaviour
{
    //id만 가지고있으면 플레이어가 id를 통해 데이터를 가져올수있음.
    public ItemData itemData;
    

    public void Start()
    {
        Invoke("init",3f);
    }

    void init()
    {
        itemData = DataManager.Instance.GetItemById(1);
    }
}
