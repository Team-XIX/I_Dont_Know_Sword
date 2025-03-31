using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : MonoBehaviour
{
    public int getId;

    public EquipItemData equipItemData;
    SpriteRenderer _spriteRenderer;

    public void Start()
    {
        Invoke("init", 3f);
    }

    void init()
    {
        equipItemData = DataManager.Instance.GetEquipItemById(getId);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = equipItemData.GetSprite();
    }
}
