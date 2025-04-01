using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

[System.Serializable]
public class test_Item
{
    public int id;
    public string name;
    public int value;

    public test_Item(int _id, string _name, int _value)
    {
        id = _id;
        name = _name;
        value = _value;
    }

}

[System.Serializable]
public class ItemList
{
    public List<test_Item> items = new List<test_Item>();
}

public class GoogleSheetToJson : MonoBehaviour
{
    // 구글 스프레드시트 CSV 링크 ("스프레드시트ID"를 본인의 ID로 변경)
    private string googleSheetUrl = "https://docs.google.com/spreadsheets/d/13LaKiWlLCM3n6clMDMPFnIkyIVu88-dFgpKQc3IAR08/gviz/tq?tqx=out:csv&gid=1249418767";

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
            Debug.Log("데이터 가져오기 성공!");
            string csvData = request.downloadHandler.text;
            Debug.Log(csvData);
            csvData = csvData.Replace("\"","");
            //"\"0\""
            string jsonData = ConvertCsvToJson(csvData);

            // JSON 저장
            string path = Path.Combine(Application.persistentDataPath, "data.json");
            File.WriteAllText(path, jsonData);

            Debug.Log("JSON 저장 완료: " + path);
        }
        else
        {
            Debug.LogError("에러: " + request.error);
        }
    }

    string ConvertCsvToJson(string csv)
    {
        string[] lines = csv.Split('\n');
        if (lines.Length <= 1) return "{}";

        // 헤더 추출
        string[] headers = lines[0].Trim().Split(',');

        // 아이템 리스트 생성
        ItemList itemList = new ItemList();
        
        // 데이터 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');
            //헤더와 데이터의 길이가 같은지 검사
            if (values.Length != headers.Length) continue;

            test_Item item = new(int.Parse(values[0]), values[1], int.Parse(values[2]));

            itemList.items.Add(item);
        }

        return JsonUtility.ToJson(itemList, true); // 포맷팅된 JSON 반환
    }
}
