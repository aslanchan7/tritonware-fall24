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
        tiles = new MapTile[MapSize.x,MapSize.y];

        CreateGameGrid();
    }

    private void CreateGameGrid()
    {
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                MapTile newTile = Instantiate(MapTilePrefab);
                newTile.transform.SetParent(GameGrid, false);
                newTile.transform.position = new Vector2(i, j);
                newTile.Pos = new Vector2Int(i, j);
                newTile.SpriteRenderer.enabled = false;
                tiles[i, j] = newTile;
            }
        }
    }

    [ExecuteInEditMode]
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

    [ExecuteInEditMode]
    public void ClearTiles()
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                Tilemap.SetTile(tilePosition, null);
            }
        }
    }
}
