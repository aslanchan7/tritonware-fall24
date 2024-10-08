using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
    public static Vector2 GetTileCenter(this Vector2Int pos)
    {
        return new Vector2(pos.x + MapManager.TileSize.x / 2, pos.y + MapManager.TileSize.y / 2);
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        degrees *= Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(degrees) - v.y * Mathf.Sin(degrees),
            v.x * Mathf.Sin(degrees) + v.y * Mathf.Cos(degrees)
        );
    }
}