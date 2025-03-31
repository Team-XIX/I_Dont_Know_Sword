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
        if(GUILayout.Button("모든 방의 통로 열기"))
        {
            MapManager.Instance.mapCreator.OpenAllRooms();
        }
        if (GUILayout.Button("모든 방의 통로 닫기"))
        {
            MapManager.Instance.mapCreator.BlockAllRooms();
        }
    }

}
