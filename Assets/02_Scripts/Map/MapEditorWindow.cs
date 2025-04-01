using UnityEditor;
using UnityEngine;


public class MapEditorWindow : EditorWindow
{
    private static MapEditorWindow mapEditorWindow;

    [MenuItem("Window/MapEditor")]
    private static void SetUp()
    {
        mapEditorWindow = CreateInstance<MapEditorWindow>();

        mapEditorWindow.titleContent = new GUIContent("Map Editor Window");

        mapEditorWindow.ShowUtility();
    }

    private void OnEnable()
    {
        
    }

    private void OnGUI()
    {
        if (GUILayout.Button("현재 방의 모든 몬스터 죽이기"))
        {
            MapManager.Instance.mapCreator.KillMostersInCurrentRoom();
        }

        if (GUILayout.Button("현재 방의 통로 열기"))
        {
            MapManager.Instance.mapCreator.OpenEnteranceInCurrentRoom();
        }

        if (GUILayout.Button("현재 방의 통로 닫기"))
        {
            MapManager.Instance.mapCreator.BlockEnteranceInCurrentRoom();
        }

        if (GUILayout.Button("허식 무라사키"))
        {
            MapManager.Instance.mapCreator.KillMosters();
        }

        if (GUILayout.Button("모든 방의 통로 열기"))
        {
            MapManager.Instance.mapCreator.OpenAllRooms();
        }

        if (GUILayout.Button("모든 방의 통로 닫기"))
        {
            MapManager.Instance.mapCreator.BlockAllRooms();
        }

        if (GUILayout.Button("바로 보스방으로 입장하기"))
        {
            MapManager.Instance.mapCreator.TeleportToBossRoom();
        }

    }

}
