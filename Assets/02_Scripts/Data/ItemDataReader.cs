using UnityEngine;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct ItemData
{
    public int id;
    public string name;
    [TextArea] public string description;

    public ItemData(int id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
    }
}

//스크립터블 오브젝트를 생성하기위한 UI를 등록합니다.
[CreateAssetMenu(fileName = "Reader", menuName = "Scriptable Object/ItemDataReader", order = int.MaxValue)]
public class ItemDataReader : DataReaderBase
{
    //시트에서 데이터를 읽으면 이 리스트에 저장됩니다.
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")][SerializeField] public List<ItemData> DataList = new List<ItemData>();

    //가장 핵심이 되는 코드로, 각 행을 읽을때마다 데이터를 저장하기위해 구조체를 생성하고, 리스트에 삽입합니다.
    //case "id"는 스프레드 시트의 열이름이 되며, 해당 열 이름이 "id"인경우 구조체의 id의 변수에 삽입되도록 하는 구조입니다.
    internal void UpdateStats(List<GSTU_Cell> list, int itemID)
    {
        int id = 0;
        string name = null, description = null;

        for (int i = 0; i < list.Count; i++)
        {
            switch (list[i].columnId)
            {
                case "id":
                    {
                        id = (int)int.Parse(list[i].value);
                        break;
                    }
                case "name":
                    {
                        name = list[i].value;
                        break;
                    }
                case "description":
                    {
                        description = list[i].value;
                        break;
                    }
            }
        }

        DataList.Add(new ItemData(id, name, description));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemDataReader))]
public class ItemDataReaderEditor : Editor
{
    ItemDataReader data;

    void OnEnable()
    {
        data = (ItemDataReader)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("\n\n스프레드 시트 읽어오기");

        if (GUILayout.Button("데이터 읽기(API 호출)"))
        {
            UpdateStats(UpdateMethodOne);
            data.DataList.Clear();
        }
    }

    void UpdateStats(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(data.associatedSheet, data.associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethodOne(GstuSpreadSheet ss)
    {
        for (int i = data.START_ROW_LENGTH; i <= data.END_ROW_LENGTH; ++i)
        {
            data.UpdateStats(ss.rows[i], i);
        }

        EditorUtility.SetDirty(target);
    }
}
#endif
