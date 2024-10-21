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
            if (!SelectedUnits.Contains(foundUnit)) 
            { 
                SelectUnit(foundUnit);
            }
            
        }
    }

    public void SelectStructure(Structure structure)
    {
        Debug.Log("Clicked on structure " + structure.name);
        if (structure is HospitalBed b && b.Patient != null)
        {
            SelectUnit(b.Patient);
        }
    }


    public void SelectUnit(Unit unit)
    {
        SelectedUnits.Add(unit);
        unit.SelectIndicator.enabled = true;
    }
    public void DeselectUnit(Unit unit)
    {
        SelectedUnits.Remove(unit);
        unit.SelectIndicator.enabled = false;
    }

    public void GiveOrder(Vector2Int pos)
    {
        if (SelectedUnits.Count <= 0 || !(SelectedUnits[0] is AlliedUnit a)) return;
        if (!MapManager.Instance.InPlayableBounds(pos)) return;
        if (MapManager.Instance.IsPassable(pos))
        {
            // valid move starts here
            foreach (var unit in SelectedUnits)
            {
                unit.ClearTasks();
                unit.TryExitBed();
            }
            if (SelectedUnits.Count == 1)   // can only give tasks to one unit at a time (for now)
            {
                Workstation ws = MapManager.Instance.GetTile(pos).ReservingWorkstation;

                if (ws != null)     // clicked on a work tile
                {
                    if (ws.TaskInProgress != null && ws.TaskInProgress.Worker != null && ws.TaskInProgress.Worker != SelectedUnits[0])
                    {
                        // someone else is going to work there
                        ws.TaskInProgress.Worker.ClearPath();
                        ws.TaskInProgress.PauseTask();
                    }
                    ws.PrepareWorkstationTask(SelectedUnits[0]);
                }
            }
            OrderMove(pos);
        }
        else if (SelectedUnits.Count == 1) 
        {
            Structure structure = MapManager.Instance.GetTile(pos).ContainedStructure;
            if (structure != null && structure.StructureTaskTemplate != null)
            {
                // valid move starts here
                foreach (var unit in SelectedUnits)
                {
                    unit.ClearTasks();
                    unit.TryExitBed();
                }
                Task task = structure.PrepareStructureTask(SelectedUnits[0]);
                if (task == null) 
                {
                    Debug.LogWarning("Structure returned null task");
                    return;
                }
                OrderMove(task.ValidWorkingPositions.GetClosest(SelectedUnits[0].Pos));
            }
        }
    }

    public void OrderMove(Vector2Int pos)
    {
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
            // this is the new code
            yield return new WaitUntil(() => SelectedUnits[0].PathSetupFinished);
            SelectedUnits[0].PathSetupFinished = false;
            for (int i = 1; i < SelectedUnits.Count; i++)
            {
                alliedUnit = (AlliedUnit)SelectedUnits[i];
                if (alliedUnit.IsControllable())
                {
                    alliedUnit.TryExitBed();
                    pos = (Vector2Int)FindFreeNeighbor(initPos, i);
                    yield return StartCoroutine(SelectedUnits[i].PathfindCoroutine(pos));
                    // this is the new code
                    yield return new WaitUntil(() => SelectedUnits[i].PathSetupFinished);
                    SelectedUnits[i].PathSetupFinished = false;
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
                        Debug.LogError("A* pathfinding needed more time to calculate the path. Look at Unit.cs under Pathfind()");
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
            foreach (Vector2Int neighbor in MapManager.Instance.GetAdjacents(currentPos))
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


}