using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public Transform slotPanel;

    public delegate void OnInventoryChanged();
    private int curEquipIndex = -1;
    private void Start()
    {
        slots = new ItemSlot[slotPanel.childCount];
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].onItemSelected += EquipItem;
        }
    }

    public void Additem()
    {
        for(int i = 0;i < slots.Length;i++)
        {
           // 슬롯 리스트에 추가되면 아이템을 자동으로 획득함
        }
    }

    public void EquipItem(int index)
    {
        if (curEquipIndex == index) return;

        if (curEquipIndex != -1)
            slots[curEquipIndex].SetEquipped(false);

        curEquipIndex = index;
        slots[curEquipIndex].SetEquipped(true);

        // UpdateUI(); UI 정보 업데이트 
    }
}
