using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int tilemapCellPosition = MapManager.Instance.Tilemap.WorldToCell(mouseWorldPos);
        Vector2Int pos = (Vector2Int)tilemapCellPosition;
        if (MapManager.Instance.Tilemap.HasTile(tilemapCellPosition))
        {
            if (Input.GetMouseButtonUp(0))
            {
                SelectTile(pos);
            }
            if (Input.GetMouseButtonUp(1))
            {
                OrderTile(pos);
            }
            HoverTile(pos);
        }


    }


    void HoverTile(Vector2Int pos)
    {
        if (MapTile.lastHovered != MapManager.Instance.GetTile(pos))
        {
            OverlayManager.Instance.HoverTile(pos);
        }
    }

    // LClick
    void SelectTile(Vector2Int pos)
    {
        UnitController.Instance.SelectUnitAtPos(pos);
    }

    // RClick
    void OrderTile(Vector2Int pos)
    {
        UnitController.Instance.GiveOrder(pos);
    }
}
