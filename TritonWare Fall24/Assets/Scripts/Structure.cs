using UnityEngine;

public abstract class Structure : Entity
{
    public virtual void Place(Vector2Int targetPos)
    {
        Pos = targetPos;
        MapTile targetTile = MapManager.Instance.GetTile(targetPos);
        transform.SetParent(MapManager.Instance.GetTile(targetPos).transform, false);
        foreach (Vector2Int occupiedPos in GetOccupiedPositions())
        {
            // fill up all tiles that are bounded by the size of this structure
            MapTile partialTile = MapManager.Instance.GetTile(occupiedPos);
            if (!partialTile.IsPassable() || partialTile.ContainedStructure != null)
            {
                Debug.LogError("Tried to place into occupied tile");
            }
            partialTile.ContainedStructure = this;
            if (BlocksMovement)
            {
                PathfindingUtils.SetWalkable(partialTile.Pos, false);
            }
        }
        
    }
}