using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapManager : MonoBehaviour
{
    public Tilemap FloorTilemap;
    public Tilemap WallTilemap;
    public TileBase TileBase;
    public static MapManager Instance;
    public MapTile MapTilePrefab;
    public PermanentWall WallPrefab;
    public PermanentWall LWallPrefab;
    public PermanentWall RWallPrefab;
    public Transform GameGrid;

    public Vector2Int MapSize = new Vector2Int(80, 50);
    public int MapBoundary = 5;

    public static float TileSize = 1;     // I hope we never need to change this
    public MapTile[,] Tiles;

    public List<TileBase> WallTiles;

    private void Awake()
    {
        Instance = this;
        InitGameGrid();
        InitPathfinder();
        GetPlayableMapEdge();
    }

    private void Update()
    {


    }



    public MapTile GetTile(Vector2Int pos)
    {
        return Tiles[pos.x, pos.y];
    }

    public Unit GetUnit(Vector2Int pos)
    {
        return GetTile(pos).ContainedUnit;
    }

    public ResourcePickup GetResource(Vector2Int pos)
    {
        return GetTile(pos).ContainedResource;
    }

    public List<MapTile> GetTilesInRadius(Vector2Int origin, float radius, bool allowOutOfBounds = false)
    {
        int roundedRadius = (int)radius + 1;
        List<MapTile> tiles = new List<MapTile>();
        // draw a square with side 2r first, then select tiles that fit in the circle
        for (int i = origin.x - roundedRadius; i <= origin.x + roundedRadius; i++)
        {
            for (int j = origin.y - roundedRadius; j <= origin.y + roundedRadius; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                bool inBounds;
                if (allowOutOfBounds) inBounds = InWorldBounds(pos);
                else inBounds = InPlayableBounds(pos);

                // +0.05 to account for floating point precision
                if (inBounds && Vector2Int.Distance(pos, origin) <= radius + 0.05f)
                {
                    tiles.Add(GetTile(pos));
                }

            }
        }
        return tiles;
    }

    public bool InPlayableBounds(Vector2Int pos)
    {
        return pos.x >= MapBoundary && pos.x < MapSize.x - MapBoundary && pos.y >= MapBoundary && pos.y < MapSize.y - MapBoundary;
    }

    public bool InWorldBounds(Vector2Int pos)
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
                    PermanentWall wall;
                    if (WallTiles.Contains(WallTilemap.GetTile((Vector3Int)pos)))
                    {
                        Matrix4x4 tileMatrix = WallTilemap.GetTransformMatrix((Vector3Int)pos);
                        if (tileMatrix.lossyScale.x > 0)
                        {
                            wall = Instantiate(LWallPrefab);
                        }
                        else
                        {
                            wall = Instantiate(RWallPrefab);
                        }
                    }
                    else
                    {
                        wall = Instantiate(WallPrefab);
                    }

                    wall.Place(pos);
                }
            }
        }
    }

    private void InitPathfinder()
    {
        // This holds all graph data
        AstarData data = AstarPath.active.data;

        // This creates a Grid Graph
        GridGraph gg = data.gridGraph;

        // Setup a grid graph with some values
        float nodeSize = 1;

        gg.center = new Vector3(MapSize.x / 2f, MapSize.y / 2f, 0);

        // Updates internal size from the above values
        gg.SetDimensions(MapSize.x, MapSize.y, nodeSize);

        // Scans all graphs
        AstarPath.active.Scan();
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

    // orthogonal adjacents only
    public List<Vector2Int> GetAdjacents(Vector2Int pos)
    {
        List<Vector2Int> adjacents = new List<Vector2Int>();
        foreach (Vector2Int toAdd in new Vector2Int[] {
            new Vector2Int(pos.x - 1, pos.y), // Left
            new Vector2Int(pos.x + 1, pos.y), // Right
            new Vector2Int(pos.x, pos.y - 1), // Down
            new Vector2Int(pos.x, pos.y + 1) })
        { // Up

            if (InPlayableBounds(toAdd)) adjacents.Add(toAdd);
        }

        return adjacents;
    }

    // neighbors including diagonals
    public List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == j && i == 0) continue;
                Vector2Int neighbor = pos + new Vector2Int(i, j);
                if (InPlayableBounds(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }

    public List<Vector2Int> GetFreeNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == j && i == 0) continue;
                Vector2Int neighbor = pos + new Vector2Int(i, j);
                if (InPlayableBounds(neighbor) && IsPassable(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }

    public static bool WorldPosWithinTile(Vector3 worldPos, Vector2Int tilePos)
    {
        return ((worldPos.x >= tilePos.x * TileSize) && (worldPos.x <= (tilePos.x + 1) * TileSize) &&
            (worldPos.y >= tilePos.y * TileSize) && (worldPos.y <= (tilePos.y + 1) * TileSize));
    }

    public List<Vector2Int> GetMapEdge()
    {
        List<Vector2Int> result = new();
        for (int i = 0; i < MapSize.x - 1; i++)
        {
            result.Add(new Vector2Int(i, 0));
        }
        for (int j = 0; j < MapSize.y - 1; j++)
        {
            result.Add(new Vector2Int(MapSize.x - 1, j));
        }
        for (int i = MapSize.x - 1; i >= 1; i--)
        {
            result.Add(new Vector2Int(i, MapSize.y - 1));
        }
        for (int j = MapSize.y - 1; j >= 1; j--)
        {
            result.Add(new Vector2Int(0, j));
        }
        return result;
    }

    public List<Vector2Int> GetPlayableMapEdge()
    {
        List<Vector2Int> result = new();
        for (int i = MapBoundary; i < MapSize.x - MapBoundary - 1; i++)
        {
            result.Add(new Vector2Int(i, MapBoundary));
        }
        for (int j = MapBoundary; j < MapSize.y - MapBoundary - 1; j++)
        {
            result.Add(new Vector2Int(MapSize.x - MapBoundary - 1, j));
        }
        for (int i = MapSize.x - MapBoundary - 1; i >= MapBoundary + 1; i--)
        {
            result.Add(new Vector2Int(i, MapSize.y - MapBoundary - 1));
        }
        for (int j = MapSize.y - MapBoundary - 1; j >= MapBoundary + 1; j--)
        {
            result.Add(new Vector2Int(MapBoundary, j));
        }

        /*
        foreach (Vector2Int pos in result)
        {
            GetTile(pos).SpriteRenderer.enabled = !GetTile(pos).SpriteRenderer.enabled;
        }
        */

        return result;
    }

}
