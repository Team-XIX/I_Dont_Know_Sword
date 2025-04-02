using UnityEngine;

public abstract class DataReaderBase : ScriptableObject
{
    //시트의 주소를 연결합니다.
    //아래의 이미지처럼 드래그한 영역 /d/ ~ /edit# 범위 내인 '~' 영역을 지정하면 됩니다.
    //'1hAEhskFqhVJhzuly7l1c6xTNfdz0m3filSbBDMC6nRk'가 들어갈 값입니다.
    [Header("시트의 주소")][SerializeField] public string associatedSheet = "";
    [Header("스프레드 시트의 시트 이름")][SerializeField] public string associatedWorksheet = "";
    [Header("읽기 시작할 행 번호")][SerializeField] public int START_ROW_LENGTH = 2;
    [Header("읽을 마지막 행 번호")][SerializeField] public int END_ROW_LENGTH = -1;
}
