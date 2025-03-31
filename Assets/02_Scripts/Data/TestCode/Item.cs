using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int getId;

    public ItemData itemData;
    SpriteRenderer _spriteRenderer;

    public void Start()
    {
        getId = Random.Range(1, DataManager.Instance.itemCount);
        init();
    }

    void init()
    {
        itemData = DataManager.Instance.GetItemById(getId);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = itemData.GetSprite();
    }
}
