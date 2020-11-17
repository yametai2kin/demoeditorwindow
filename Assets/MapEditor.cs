using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// マップエディタ―
///		ExecuteInEditMode属性を付与する事で非Play状態でも動作する
/// </summary>
[ExecuteInEditMode]
public class MapEditor : MonoBehaviour
{
    public GameObject cursor = null;
    public GameObject pickPlane = null;
    public GameObject parentBlock = null;
    public float gridSize = 1;

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        //シーンビュー上のイベントを取得するため、メソッドを登録
        SceneView.onSceneGUIDelegate += OnOccurredEventOnSceneView;
    }

    /// <summary>
    /// シーンビュー上でのイベントをハンドリング
    /// </summary>
    /// <param name="scene"></param>
    void OnOccurredEventOnSceneView( SceneView sceneView )
    {
        // ブロック選択中なら何もしない
        if( Selection.activeGameObject && Selection.activeGameObject.name.Contains( "Block" ) )
        {
            return;
        }

        // マウスをクリックしたグリッド上にカーソルを移動する
        if( Event.current.type == EventType.MouseDown )
        {
            // 処理中のイベントからマウスの位置取得(Y軸が反転してる？ので補正する)
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;

            // レイを飛ばしてXZ表面上での位置を得る
            Ray ray = sceneView.camera.ScreenPointToRay( mousePosition );
            RaycastHit hit;
            if( Physics.Raycast( ray, out hit, Mathf.Infinity, LayerMask.GetMask( "Editor(Pick)" ) ) )
            {
                float halfGrid = this.gridSize / 2;
                Vector3 pos = new Vector3(
                    ( int )( hit.point.x / this.gridSize ) * this.gridSize + halfGrid,
                    0,
                    ( int )( hit.point.z / this.gridSize ) * this.gridSize + halfGrid
                );

                if( hit.point.x < 0 )
                {
                    pos.x -= 1;
                }
                if( hit.point.z < 0 )
                {
                    pos.z -= 1;
                }

                this.cursor.transform.position = pos;
            }
        }
    }

    /// <summary>
    /// シーンビュー、ゲームビューで更新があった場合(Play中とは挙動が違います)
    /// </summary>
    void Update()
    {
        if( Selection.activeGameObject )
        {
            // ブロックを選択しているなら座標をグリッドに合わせる
            if( Selection.activeGameObject.name.Contains( "Block" ) )
            {
                AdjustGrid( Selection.activeGameObject.transform );
            }
        }
    }

    /// <summary>
    /// グリッドに合わせる
    /// </summary>
    /// <param name="transform"></param>
    void AdjustGrid( Transform transform )
    {
        float halfGrid = this.gridSize / 2;
        Vector3 pos = new Vector3(
            ( int )( transform.position.x / this.gridSize ) * this.gridSize + halfGrid,
            0.5f,
            ( int )( transform.position.z / this.gridSize ) * this.gridSize + halfGrid
        );
        transform.position = pos;
    }

    /// <summary>
    /// ブロックの追加
    /// </summary>
    public void AddBlock( string name )
    {
        GameObject srcBlock = Resources.Load<GameObject>( name );
        if( srcBlock != null )
        {
            GameObject newBlock = Instantiate<GameObject>( srcBlock );
            newBlock.transform.position = this.cursor.transform.position;
            newBlock.transform.parent = this.parentBlock.transform;
            AdjustGrid( newBlock.transform );
        }
    }

    /// <summary>
    /// ブロックのクリア
    /// </summary>
    public void ClearBlock()
    {
        if( this.parentBlock )
        {
            for( int i = this.parentBlock.transform.childCount - 1 ; i >= 0 ; i-- )
            {
                GameObject.DestroyImmediate( this.parentBlock.transform.GetChild( i ).gameObject );
            }

        }
    }

}
