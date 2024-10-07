using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class MapManager : MonoBehaviour
{
    public Tilemap Tilemap;
    public TileBase TileBase;
    public static MapManager Instance;
    public MapTile MapTilePrefab;
    public Transform GameGrid;

    public Vector2Int MapSize = new Vector2Int(80, 50);
    private MapTile[,] tiles;



    private void Awake()
    {
        Instance = this;
        CreateGameGrid();
    }



    public MapTile GetTile(Vector2Int pos)
    {
        return tiles[pos.x,pos.y];
    }

    public Unit GetUnit(Vector2Int pos)
    {
        return GetTile(pos).ContainedUnit;
    }

    public Vector3 GetWorldPos(Vector2Int pos)
    {
        return Tilemap.CellToWorld((Vector3Int)pos);
    }

    public bool IsPassable(Vector2Int pos)
    {
        return GetTile(pos).ContainedUnit == null;
        // todo structures
    }


    public void CreateTileMap()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                Tilemap.SetTile(tilePosition, TileBase);
            }
        }
    }

    public void CreateGameGrid()
    {
        if (tiles != null)
        {
            Debug.LogWarning("Game Grid already exists");
            return;
        }
        tiles = new MapTile[MapSize.x, MapSize.y];
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                MapTile newTile = Instantiate(MapTilePrefab);
                newTile.gameObject.name = $"Tile {i}, {j}";
                newTile.transform.SetParent(GameGrid, false);
                newTile.transform.position = new Vector2(i, j);
                newTile.Pos = new Vector2Int(i, j);
                newTile.SpriteRenderer.enabled = false;
                tiles[i, j] = newTile;
            }
        }
    }

    public void ClearMap()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                Tilemap.SetTile(tilePosition, null);
            }
        }
        while (GameGrid.childCount > 0)
        {
            DestroyImmediate(GameGrid.GetChild(0).gameObject);
        }
        tiles = null;
    }

}
