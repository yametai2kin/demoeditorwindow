using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// マップカスタムエディタ
/// </summary>
public class MapEditorWindow : EditorWindow
{
    public MapEditor mapEditor = null;

    /// <summary>
    /// GUI更新
    /// </summary>
    void OnGUI()
    {
        if( GUILayout.Button( "Add Cube" ) )
        {
            this.mapEditor.AddBlock( "BlockCube" );
        }
        if( GUILayout.Button( "Add Sphere" ) )
        {
            this.mapEditor.AddBlock( "BlockSphere" );
        }
        if( GUILayout.Button( "Clear All" ) )
        {
            this.mapEditor.ClearBlock();
        }

    }

    /// <summary>
    /// ウィンドウを表示する
    /// </summary>
    [MenuItem( "Window/Map Editor" )]
    public static void ShowWindow()
    {
        MapEditorWindow window = EditorWindow.GetWindow<MapEditorWindow>();
        window.mapEditor = FindObjectOfType<MapEditor>();
    }
}
