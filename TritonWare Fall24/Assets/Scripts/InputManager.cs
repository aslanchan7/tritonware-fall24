using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    private Vector2Int startPos;

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int tilemapCellPosition = MapManager.Instance.FloorTilemap.WorldToCell(mouseWorldPos);
        Vector2Int pos = (Vector2Int)tilemapCellPosition;
        if (MapManager.Instance.FloorTilemap.HasTile(tilemapCellPosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Clear the previous selected units 
                foreach (Unit unit in UnitController.Instance.SelectedUnits)
                {
                    unit.SelectIndicator.enabled = false;
                }
                UnitController.Instance.SelectedUnits.Clear();

                // Store the position of the initial press of LClick
                startPos = pos;
            }

            // Update the hover overlay while left mouse button is being held down
            if (Input.GetMouseButton(0))
            {
                OverlayManager.Instance.SelectTiles(startPos, pos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (startPos == pos)    // single tile
                {
                    SelectTile(pos);
                }
                List<Unit> foundUnits = MapManager.Instance.FindUnitsInArea(startPos, pos);
                foreach (var unit in foundUnits)
                {
                    if (unit is AlliedUnit) SelectTile(unit.Pos);
                }

                OverlayManager.Instance.ResetHoverOverlay();
            }

            if (Input.GetMouseButtonUp(1))
            {
                OrderTile(pos);
            }

            // Only HoverTile(pos) when LClick is not being held down
            if (!Input.GetMouseButton(0))
            {
                HoverTile(pos);
            }
        }
        else
        {
            OverlayManager.Instance.ResetHoverOverlay();
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
        if (MapManager.Instance.GetTile(pos).ContainedStructure != null)
        {
            UnitController.Instance.SelectStructure(MapManager.Instance.GetTile(pos).ContainedStructure);
        }
    }

    // RClick
    void OrderTile(Vector2Int pos)
    {
        UnitController.Instance.GiveOrder(pos);
    }
}
