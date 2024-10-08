using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class MapManager : MonoBehaviour
{
    public Tilemap FloorTilemap;
    public Tilemap WallTilemap;
    public TileBase TileBase;
    public static MapManager Instance;
    public MapTile MapTilePrefab;
    public PermanentWall WallPrefab;
    public Transform GameGrid;

    public Vector2Int MapSize = new Vector2Int(80, 50);
    public static float TileSize = 1;     // I hope we never need to change this
    public MapTile[,] Tiles;



    private void Awake()
    {
        Instance = this;
        InitGameGrid();
    }



    public MapTile GetTile(Vector2Int pos)
    {
        return Tiles[pos.x, pos.y];
    }

    public Unit GetUnit(Vector2Int pos)
    {
        return GetTile(pos).ContainedUnit;
    }

    public List<MapTile> GetTilesInRadius(Vector2Int origin, float radius)
    {
        int roundedRadius = (int)radius + 1;
        List<MapTile> tiles = new List<MapTile>();
        // draw a square with side 2r first, then select tiles that fit in the circle
        for (int i = origin.x - roundedRadius; i <= origin.x + roundedRadius; i++)
        {
            for (int j = origin.y - roundedRadius; j <= origin.y + roundedRadius; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                // +0.05 to account for floating point precision
                if (InBounds(pos) && Vector2Int.Distance(pos, origin) <= radius + 0.05f)
                {
                    tiles.Add(GetTile(pos));
                }     

            }
        }
        return tiles;
    }

    public bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < MapSize.x && pos.y >= 0 && pos.y < MapSize.y;
    }

    public Vector3 GetWorldPos(Vector2Int pos)
    {
        return FloorTilemap.CellToWorld((Vector3Int)pos);
    }

    public bool IsPassable(Vector2Int pos)
    {
        return GetTile(pos).IsPassable();
    }


    public void CreateTileMap()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                FloorTilemap.SetTile(tilePosition, TileBase);
            }
        }
    }

    public void InitGameGrid()
    {
        if (Tiles != null)
        {
            Debug.LogWarning("Game Grid already exists");
            return;
        }
        Tiles = new MapTile[MapSize.x, MapSize.y];
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                MapTile newTile = Instantiate(MapTilePrefab);
                newTile.gameObject.name = $"Tile {i}, {j}";
                newTile.transform.SetParent(GameGrid, false);
                newTile.transform.position = (Vector2)pos * TileSize;
                newTile.Pos = pos;
                newTile.SpriteRenderer.enabled = false;
                Tiles[i, j] = newTile;

                if (WallTilemap.HasTile((Vector3Int)pos))
                {
                    Instantiate(WallPrefab).Place(pos);
                }
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
                FloorTilemap.SetTile(tilePosition, null);
            }
        }
        while (GameGrid.childCount > 0)
        {
            DestroyImmediate(GameGrid.GetChild(0).gameObject);
        }
        Tiles = null;
    }

    public List<Unit> FindUnitsInArea(Vector2Int pos1, Vector2Int pos2)
    {
        int minX = Mathf.Min(pos1.x, pos2.x);
        int maxX = Mathf.Max(pos1.x, pos2.x);
        int minY = Mathf.Min(pos1.y, pos2.y);
        int maxY = Mathf.Max(pos1.y, pos2.y);

        List<Unit> foundUnits = new();

        if (pos1 == pos2)
        {
            Unit unit = GetTile(pos1).ContainedUnit;
            if (unit != null)
            {
                foundUnits.Add(unit);
                return foundUnits;
            }
        }

        for (int i = minX; i < maxX + 1; i++)
        {
            for (int j = minY; j < maxY + 1; j++)
            {
                Unit unit = GetTile(new Vector2Int(i, j)).ContainedUnit;
                if (unit != null)
                {
                    foundUnits.Add(unit);
                }
            }
        }

        return foundUnits;
    }

    public static Vector2 GetTileCenter(Vector2Int pos)
    {
        return new Vector2(pos.x + 0.5f, pos.y + 0.5f);
    }

}
