using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.IO;
using Unity.VisualScripting;

public class DataManager : SingleTon<DataManager>
{  
    //JSON 저장용 Wrapper 클래스
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;

        public Wrapper(List<T> items)
        {
            this.items = items;
        }
    }

    public List<ItemData> itemDataList = new List<ItemData>();
    public List<EquipItemData> equipItemDataList = new List<EquipItemData>();
    public List<WeaponData> weaponDataList = new List<WeaponData>();

    public GameObject item;
    public GameObject equipItem;
    public GameObject weaponItem;

    private string itemJsonPath;
    private string equipItemDataJsonPath;
    private string monsterJsonPath;
    private string weaponJsonPath;

    private string itemUrl = "https://docs.google.com/spreadsheets/d/13LaKiWlLCM3n6clMDMPFnIkyIVu88-dFgpKQc3IAR08/gviz/tq?tqx=out:csv&gid=1249418767";
    private string equipItemUrl = "https://docs.google.com/spreadsheets/d/13LaKiWlLCM3n6clMDMPFnIkyIVu88-dFgpKQc3IAR08/gviz/tq?tqx=out:csv&gid=1091773342";
    //private string monsterUrl = "https://docs.google.com/spreadsheets/d/13LaKiWlLCM3n6clMDMPFnIkyIVu88-dFgpKQc3IAR08/gviz/tq?tqx=out:csv&gid=1227733515";
    private string weaponUrl = "https://docs.google.com/spreadsheets/d/13LaKiWlLCM3n6clMDMPFnIkyIVu88-dFgpKQc3IAR08/gviz/tq?tqx=out:csv&gid=430856127";

    public int itemCount { get; private set; }
    public int equipItemCount { get; private set; }
    public int enemyCount { get; private set; }
    public int weaponCount { get; private set; }

    private void Awake()
    {
        //불러올 경로 설정
        itemJsonPath = Path.Combine(Application.persistentDataPath, "itemData.json");
        equipItemDataJsonPath = Path.Combine(Application.persistentDataPath, "equipItemData.json");
        monsterJsonPath = Path.Combine(Application.persistentDataPath, "monsterData.json");
        weaponJsonPath = Path.Combine(Application.persistentDataPath, "weaponData.json");

        if (transform.root != null || transform.parent != null)
            DontDestroyOnLoad(transform.root);
    }

    void Start()
    {
        StartCoroutine(DownloadAndSaveData<ItemData>(itemUrl, itemJsonPath, itemDataList));
        StartCoroutine(DownloadAndSaveData<EquipItemData>(equipItemUrl, equipItemDataJsonPath, equipItemDataList));
        StartCoroutine(DownloadAndSaveData<WeaponData>(weaponUrl, weaponJsonPath, weaponDataList));
        //StartCoroutine(DownloadAndSaveData<EnemyData>(monsterUrl, monsterJsonPath, enemyDataList));
    }

    // 데이터 다운로드 후 JSON으로 저장
    private IEnumerator DownloadAndSaveData<T>(string url, string jsonPath, List<T> dataList) where T : BaseData, new()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string csvData = request.downloadHandler.text;
            csvData = csvData.Replace("\"", "");
            Debug.Log(csvData);
            ParseCsvToData(csvData, dataList);

            // JSON으로 변환 후 파일 저장
            string json = JsonUtility.ToJson(new Wrapper<T>(dataList), true);
            File.WriteAllText(jsonPath, json);
            Debug.Log($"{typeof(T).Name} 데이터 저장 완료: {jsonPath}");

        }
        else
        {
            Debug.LogError($"{typeof(T).Name} 데이터 다운로드 실패: {request.error}");

            // JSON 파일에서 로드 시도
            LoadDataFromJson(jsonPath, dataList);
        }
    }

    // JSON에서 데이터 로드
    private void LoadDataFromJson<T>(string jsonPath, List<T> dataList) where T : BaseData
    {
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            dataList.AddRange(wrapper.items);
            Debug.Log($"{typeof(T).Name} JSON에서 로드 완료: {dataList.Count}개");
        }
        else
        {
            Debug.LogWarning($"{typeof(T).Name} JSON 파일이 존재하지 않습니다: {jsonPath}");
        }
    }

    // CSV → 데이터 변환
    private void ParseCsvToData<T>(string csv, List<T> dataList) where T : BaseData, new()
    {
        string[] lines = csv.Split('\n');

        if (lines.Length < 2) return;

        string[] headers = lines[0].Trim().Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');
            if (values.Length == 0 || string.IsNullOrWhiteSpace(values[0])) continue;

            T data = new T();
            data.SetData(headers, values);
            dataList.Add(data);
            SetCount();
        }
    }

    //아이템 조회
    public ItemData GetItemById(int id)
    {
        //Dictionary
        return itemDataList.Find(item => item.id == id);
    }
    public EquipItemData GetEquipItemById(int id)
    {
        //Dictionary
        return equipItemDataList.Find(equipItem => equipItem.id == id);
    }

    //public EnemyData GetMonsterById(int id)
    //{
    //    return enemyDataList.Find(monster => monster.id == id);
    //}
    public WeaponData GetWeaponById(int id)
    {
        return weaponDataList.Find(weapon => weapon.id == id);
    }
    public void CreateItem() // called at button
    {
        //Instantiate(item,transform);
        //Instantiate(equipItem,transform);
        Instantiate(weaponItem, transform);
    }

    private void SetCount()
    {
        //+1 for Random.Range
        itemCount = itemDataList.Count +1;
        equipItemCount = equipItemDataList.Count +1;
        weaponCount = weaponDataList.Count +1;
        //enemyCount = enemyDataList.Count +1;
    }
  
}
