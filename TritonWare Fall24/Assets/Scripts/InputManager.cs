using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectTile();
        }

    }


    void HoverTile()
    {
        // Get the mouse position in world space
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Reset the z-axis to 0 (since the tilemap is 2D)

        // Convert the world position to the corresponding cell position in the grid
        Vector3Int cellPosition = MapManager.Instance.Tilemap.WorldToCell(mouseWorldPos);

        // Check if the tile exists at this cell position
        if (MapManager.Instance.Tilemap.HasTile(cellPosition))
        {
            
        }
    }

    void SelectTile()
    {
        // Get the mouse position in world space
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Reset the z-axis to 0 (since the tilemap is 2D)

        // Convert the world position to the corresponding cell position in the grid
        Vector3Int cellPosition = MapManager.Instance.Tilemap.WorldToCell(mouseWorldPos);

        // Check if the tile exists at this cell position
        if (MapManager.Instance.Tilemap.HasTile(cellPosition))
        {
            Debug.Log("Tile clicked at: " + cellPosition);
            // Do something when the tile is clicked, e.g., change the tile, highlight, etc.
        }
    }
}
