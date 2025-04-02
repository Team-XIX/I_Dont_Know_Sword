using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject equipItem;
    public GameObject item;
    public GameObject weaponItem;
    public GameObject monsterprefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && equipItem != null)
            Instantiate(equipItem, transform);
        if (Input.GetKeyDown(KeyCode.P) && item != null)
            Instantiate(item,transform);
        if (Input.GetKeyDown(KeyCode.P) && weaponItem != null)
            Instantiate(weaponItem, transform);
        if (Input.GetKeyDown(KeyCode.P) && monsterprefab != null)
            Instantiate(monsterprefab, transform);
    }
}
