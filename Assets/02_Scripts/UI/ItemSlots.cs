using UnityEngine;
using UnityEditor.UI;
using TMPro;
using UnityEngine.UI;
using System;

public class ItemSlot : MonoBehaviour
{
   // public ItemData item; 아이템 데이터

 
    public Image icon;
    public TextMeshProUGUI AmmoText;
    private Outline outline;

    public GameObject equippedIndicator; // 장착된 아이템 표시
    public event Action<int> onItemSelected; // 슬롯 선택 이벤트

    public int index;
    public bool equipped;
  
    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    public void SetItem()
    {
        icon.gameObject.SetActive(true);
        // icon.sprite = item.icon;

        if (outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        // item = null;
        icon.gameObject.SetActive(false);
        AmmoText.text = string.Empty;
    }

    public void SwapWeapon()
    {
        onItemSelected?.Invoke(index); // 해당 슬롯 선택
    }

    public void SetEquipped(bool equipped)
    {
        equippedIndicator.SetActive(equipped);
    }
}

