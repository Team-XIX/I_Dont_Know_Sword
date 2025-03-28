using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public int value;

    public Item(int _id, string _name, int _value)
    {
        id = _id;
        name = _name;
        value = _value;
    }

}

[System.Serializable]
public class ItemList
{
    public List<Item> items = new List<Item>();
}

public class GoogleSheetToJson : MonoBehaviour
{
    // ���� ���������Ʈ CSV ��ũ ("���������ƮID"�� ������ ID�� ����)
    private string googleSheetUrl = "https://docs.google.com/spreadsheets/d/13LaKiWlLCM3n6clMDMPFnIkyIVu88-dFgpKQc3IAR08/gviz/tq?tqx=out:csv&gid=2005723570";

    void Start()
    {
        StartCoroutine(DownloadSheetData());
    }

    IEnumerator DownloadSheetData()
    {
        UnityWebRequest request = UnityWebRequest.Get(googleSheetUrl);
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("������ �������� ����!");
            string csvData = request.downloadHandler.text;
            Debug.Log(csvData);
            csvData = csvData.Replace("\"","");
            //"\"0\""
            string jsonData = ConvertCsvToJson(csvData);

            // JSON ����
            string path = Path.Combine(Application.persistentDataPath, "data.json");
            File.WriteAllText(path, jsonData);

            Debug.Log("JSON ���� �Ϸ�: " + path);
        }
        else
        {
            Debug.LogError("����: " + request.error);
        }
    }

    string ConvertCsvToJson(string csv)
    {
        string[] lines = csv.Split('\n');
        if (lines.Length <= 1) return "{}";

        // ��� ����
        string[] headers = lines[0].Trim().Split(',');

        // ������ ����Ʈ ����
        ItemList itemList = new ItemList();
        
        // ������ �Ľ�
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');
            //����� �������� ���̰� ������ �˻�
            if (values.Length != headers.Length) continue;

            Item item = new(int.Parse(values[0]), values[1], int.Parse(values[2]));

            itemList.items.Add(item);
        }

        return JsonUtility.ToJson(itemList, true); // �����õ� JSON ��ȯ
    }
}
