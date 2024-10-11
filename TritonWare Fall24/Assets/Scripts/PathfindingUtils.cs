using Pathfinding;
using UnityEngine;

public class TileReservation
{


    public Vector2Int Pos;
    public int Penalty;

    public TileReservation(Vector2Int pos, int penalty)
    {
        Pos = pos;
        Penalty = penalty;
    }
}

public static class PathfindingUtils
{
    public static void SetWalkable(Vector2Int pos, bool walkable)
    {
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {
            GridGraph gg = AstarPath.active.data.gridGraph;

            var node = gg.GetNode(pos.x, pos.y);
            node.Walkable = walkable;
            gg.CalculateConnectionsForCellAndNeighbours(pos.x, pos.y);
            // Recalculate all grid connections
            // This is required because we have updated the walkability of some nodes
            

            // If you are only updating one or a few nodes you may want to use
            // gg.CalculateConnectionsForCellAndNeighbours only on those nodes instead for performance.
        }));
    }
    public static void SetPenalty(Vector2Int pos, uint penalty)
    {
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {
            GridGraph gg = AstarPath.active.data.gridGraph;

            var node = gg.GetNode(pos.x, pos.y);
            node.Penalty = penalty;
        }));
    }

    public static void ChangePenalty(Vector2Int pos, int penalty)
    {
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {
            GridGraph gg = AstarPath.active.data.gridGraph;

            var node = gg.GetNode(pos.x, pos.y);
            node.Penalty = (uint)((int)node.Penalty + penalty);
            if (node.Penalty > 10000000) 
            {
                Debug.LogWarning("Possible overflow");
            }
        }));
    }

    public static GridNodeBase GetNode(Vector2Int pos)
    {
        GridGraph gg = AstarPath.active.data.gridGraph;

        return gg.GetNode(pos.x, pos.y);
    }
}