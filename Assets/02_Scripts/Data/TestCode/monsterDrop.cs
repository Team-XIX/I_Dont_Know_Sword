using System.Collections.Generic;
using UnityEngine;

public class monsterDrop : MonoBehaviour
{
    public GameObject[] dropItems;
    private void Start()
    {
        dropItems = Resources.LoadAll<GameObject>("PrefabItem");
    }
    private void OnDisable()
    {
        //RandomDrop();
    }

    public void RandomDrop()
    {
        int randNum = Random.Range(0, 101);
        int rand = Random.Range(0, dropItems.Length); 
        Instantiate(dropItems[rand], transform.position, Quaternion.identity);
    }
}
