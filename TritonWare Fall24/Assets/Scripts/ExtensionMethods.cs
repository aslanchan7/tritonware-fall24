using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
    public static Vector2 GetTileCenter(this Vector2Int pos)
    {
        return new Vector2(pos.x + MapManager.TileSize / 2, pos.y + MapManager.TileSize / 2);
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        degrees *= Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(degrees) - v.y * Mathf.Sin(degrees),
            v.x * Mathf.Sin(degrees) + v.y * Mathf.Cos(degrees)
        );
    }

    public static Vector2Int GetGridPos(this Vector3 position)
    {
        return new Vector2Int((int)position.x, (int)position.y);
    }

    public static T RandomElement<T>(this List<T> list)
    {
        if (list.Count == 0) return default(T);
        return list[Random.Range(0, list.Count)];
    }

}