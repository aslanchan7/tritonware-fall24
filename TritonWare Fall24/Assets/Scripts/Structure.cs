using System.Collections.Generic;
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


    public float DistanceToStructure(Vector2Int target)
    {
        float minDist = Vector2Int.Distance(Pos, target);
        foreach (Vector2Int tile in GetOccupiedPositions())
        {
            float dist = Vector2Int.Distance(tile, target);
            if (dist < minDist)
            {
                minDist = dist;
            }
        }
        return minDist;
    }

    public List<Vector2Int> GetFreeSurroundingTiles()
    {
        List<Vector2Int> result = new();
        foreach (Vector2Int occupiedPos in GetOccupiedPositions())
        {
            foreach (Vector2Int freeNeighbor in MapManager.Instance.GetFreeNeighbors(occupiedPos))
            {
                if (!result.Contains(freeNeighbor)) result.Add(freeNeighbor);
            }
        }
        return result;
    }

}