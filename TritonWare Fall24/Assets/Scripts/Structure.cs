using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : Entity
{
    public Task StructureTaskTemplate = null;   // task performed by walking up to structure e.g. getting in bed
    
    public Task PrepareStructureTask(Unit worker)
    {
        if (StructureTaskTemplate == null) return null;
        if (GetSurroundingTiles(false).Count == 0) return null;
        Task task = StructureTaskTemplate.CreateTask(this);
        task.ValidWorkingPositions.AddRange(GetSurroundingTiles(false));
        task.transform.SetParent(transform, false);
        task.AssignTask(worker);
        return task;    
    }
    
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

    public List<Vector2Int> GetSurroundingTiles(bool mustBeFree = true)
    {
        List<Vector2Int> result = new();
        foreach (Vector2Int occupiedPos in GetOccupiedPositions())
        {
            List<Vector2Int> neighbors = MapManager.Instance.GetNeighbors(occupiedPos);

            foreach (Vector2Int neighbor in neighbors)
            {
                if (!result.Contains(neighbor)) 
                {
                    if (mustBeFree)
                    {
                        if (MapManager.Instance.GetTile(neighbor).IsPassable()) result.Add(neighbor);
                    }
                    else
                    {
                        if (MapManager.Instance.GetTile(neighbor).ContainedStructure == null) result.Add(neighbor);
                    }
                }
                
            }
        }
        return result;
    }

}