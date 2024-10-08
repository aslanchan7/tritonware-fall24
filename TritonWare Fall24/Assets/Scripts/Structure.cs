using UnityEngine;

public abstract class Structure : Entity
{
    public void Place(Vector2Int targetPos)
    {
        MapTile targetTile = MapManager.Instance.GetTile(targetPos);
        if (!targetTile.IsPassable() || targetTile.ContainedStructure != null)
        {
            Debug.LogError("Tried to place into occupied tile");
        }
        transform.SetParent(MapManager.Instance.GetTile(targetPos).transform, false);
        targetTile.ContainedStructure = this;
        Pos = targetPos;
    }
}