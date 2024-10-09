using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public List<Unit> SelectedUnits;
    public static UnitController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectUnitAtPos(Vector2Int pos)
    {
        Unit foundUnit = MapManager.Instance.GetUnit(pos);
        if (foundUnit != null)
        {
            SelectedUnits.Add(foundUnit);
            foundUnit.SelectIndicator.enabled = true;
        }
    }

    public void GiveOrder(Vector2Int pos)
    {
        if (MapManager.Instance.IsPassable(pos) && SelectedUnits.Count > 0
            && SelectedUnits[0] is AlliedUnit a)
        {
            foreach (var unit in SelectedUnits)
            {
                unit.ClearTasks();
            }
            if (SelectedUnits.Count == 1)   // can only give tasks to one unit at a time (for now)
            {
                Workstation ws = MapManager.Instance.GetTile(pos).ReservingWorkstation;
                if (ws != null)     // clicked on a work tile
                {
                    ws.PrepareTask(SelectedUnits[0]);
                }
            }
            OrderMove(pos);
        }
    }

    public void OrderMove(Vector2Int pos)
    {
        // Vector2Int initPos = pos;
        // // This is just used to check if selected unit is controllable
        // AlliedUnit alliedUnit = (AlliedUnit)SelectedUnits[0];
        // if (alliedUnit.IsControllable())
        // {
        //     // SelectedUnits[0].Move(initPos);
        //     SelectedUnits[0].Pathfind(initPos);
        //     for (int i = 1; i < SelectedUnits.Count; i++)
        //     {
        //         alliedUnit = (AlliedUnit)SelectedUnits[i];
        //         if (alliedUnit.IsControllable())
        //         {
        //             pos = (Vector2Int)FindFreeNeighbor(initPos, i);
        //             // SelectedUnits[i].Move(pos);
        //             SelectedUnits[i].Pathfind(pos);
        //         }
        //     }
        // }
        StartCoroutine(OrderMoveCoroutine(pos));
    }

    private IEnumerator OrderMoveCoroutine(Vector2Int pos)
    {
        Vector2Int initPos = pos;
        // This is just used to check if selected unit is controllable
        AlliedUnit alliedUnit = (AlliedUnit)SelectedUnits[0];
        if (alliedUnit.IsControllable())
        {
            yield return StartCoroutine(SelectedUnits[0].PathfindCoroutine(initPos));
            for (int i = 1; i < SelectedUnits.Count; i++)
            {
                alliedUnit = (AlliedUnit)SelectedUnits[i];
                if (alliedUnit.IsControllable())
                {
                    pos = (Vector2Int)FindFreeNeighbor(initPos, i);
                    yield return StartCoroutine(SelectedUnits[i].PathfindCoroutine(pos));
                }
            }
        }
    }

    private Vector2Int? FindFreeNeighbor(Vector2Int pos, int index)
    {
        Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
        toCheck.Enqueue(pos);

        while (toCheck.Count > 0)
        {
            Vector2Int currentPos = toCheck.Dequeue();
            MapTile currentTile = MapManager.Instance.GetTile(currentPos);

            // Check if the current box is free
            if (currentTile.ContainedUnit == null && MapManager.Instance.IsPassable(currentPos))
            {
                bool occupied = false;
                for (int j = 0; j < index; j++)
                {
                    // This is just for debugging purposes if something goes wrong
                    if (SelectedUnits[j].CurrentPath == null)
                    {
                        Debug.LogError("A* pathfinding needed more time to calculate the path. Look at UnitController.cs:50");
                    }

                    if (currentPos.Equals(SelectedUnits[j].CurrentPath.vectorPath[^1].GetGridPos()))
                    {
                        occupied = true;
                        break;
                    }
                }

                if (!occupied) return currentPos; // Found a free box
            }

            // Add neighboring positions to the queue
            foreach (Vector2Int neighbor in GetNeighbors(currentPos))
            {
                if (neighbor.x >= 0 && neighbor.x < MapManager.Instance.MapSize.x &&
                    neighbor.y >= 0 && neighbor.y < MapManager.Instance.MapSize.y)
                {
                    toCheck.Enqueue(neighbor);
                }
            }
        }

        // No free box found
        Debug.LogError("Literally every single tile is occupied!?!");
        return null;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
    {
        new Vector2Int(pos.x - 1, pos.y), // Left
        new Vector2Int(pos.x + 1, pos.y), // Right
        new Vector2Int(pos.x, pos.y - 1), // Down
        new Vector2Int(pos.x, pos.y + 1)  // Up
    };

        return neighbors;
    }
}