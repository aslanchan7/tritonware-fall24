using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Collections;
using UnityEditor.Experimental.GraphView;


public abstract class Unit : Entity, IDamageable
{
    public override bool BlocksMovement => true;
    public override bool BlocksVision => false;
    public override Vector2Int Size => new Vector2Int(1, 1);

    // Stats
    public int Health = 100;
    public int MaxHealth = 100;
    public float Speed = 5f;

    // Tasks
    public List<Task> TaskQueue = new List<Task>();     // stores the immediate next task at index 0


    public SpriteRenderer SelectIndicator;
    public UnitDisplay UnitDisplay;

    // Pathfinding
    public Seeker Seeker;
    public Path CurrentPath;
    [HideInInspector] public bool ReachedEndOfPath;
    private int currentWaypoint = 0;
    private float nextWaypointDistance = .2f;

    public void Awake()
    {
        Seeker = GetComponent<Seeker>();
    }


    public void EnqueueTask(Task task)          // instantiated (non template) classes only!
    {
        if (task.IsTemplate) Debug.LogError("Trying to assign template task!");
        TaskQueue.Add(task);
    }

    public void ClearTasks()                    // pauses all ongoing tasks and remove from queue
    {
        while (TaskQueue.Count > 0)
        {
            TaskQueue[0].PauseTask();
        }
    }

    public bool RemoveTask(Task task)           // unassigns this task from this unit but do not destroy it
    {
        return TaskQueue.Remove(task);
    }

    public Task GetCurrentTask()
    {
        if (TaskQueue.Count > 0) { return TaskQueue[0]; }
        else return null;
    }

    private void CheckWorkableTask()    // on arriving at move destination, see if the enqueued task is workable, and start working if yes
    {
        Task currentTask = GetCurrentTask();
        if (currentTask != null)
        {
            if (currentTask.ValidWorkingPositions.Contains(Pos))
            {
                currentTask.StartTask();
            }
        }
    }

    public IEnumerator PathfindCoroutine(Vector2Int targetPos)
    {
        yield return StartCoroutine(Pathfind(targetPos));
    }

    // Pathfinding should be done as a series of Move() calls
    private IEnumerator Pathfind(Vector2Int targetPos)
    {
        Vector2 startPos = Pos.GetTileCenter();
        Vector2 targetPosCenter = targetPos.GetTileCenter();
        Seeker.StartPath(startPos, targetPosCenter, OnPathComplete);

        // Wait for a small amount of time for A* algo to do its thing
        // Usually A* takes 3-4ms to calculate the first time then the rest are almost instant (0.0ms)
        // Note: this is a lil janky/hacky but works quite well
        yield return new WaitForSeconds(0.05f);
    }

    // Finalizes moves a unit from one tile to another
    // In game this should normally be called for neighboring tiles only
    public void Move(Vector2Int targetPos)
    {
        if (targetPos == Pos) return;
        MapTile targetTile = MapManager.Instance.GetTile(targetPos);
        if (!targetTile.IsPassable())
        {
            Debug.LogWarning(this.gameObject.name + " tried to move into occupied tile");
        }
        MapManager.Instance.GetTile(Pos).ContainedUnit = null;
        transform.SetParent(targetTile.transform, false);
        targetTile.ContainedUnit = this;
        Pos = targetPos;
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            CurrentPath = p;

            // currentWaypoint skips path's 1st tile because the 1st tile is the current tile that the unit is on
            currentWaypoint = 1;
        }
        else
        {
            Debug.LogError("Error while finding path using A*");
        }
    }

    public void Damage(int value)
    {
        Health -= value;
        // todo death

        UnitDisplay.UpdateDisplay();
    }


    public List<Unit> GetUnitsInRadius(float radius)
    {
        List<Unit> result = new List<Unit>();
        foreach (MapTile tile in MapManager.Instance.GetTilesInRadius(Pos, radius))
        {
            if (tile.ContainedUnit != null)
            {
                result.Add(tile.ContainedUnit);
            }
        }
        return result;
    }

    // returns true if destination is reached
    public bool MoveAlongPath()
    {
        if (CurrentPath == null) return false;

        ReachedEndOfPath = false;
        // Find the center position of the unit
        Vector3 unitPosCenter = new(transform.position.x + 0.5f, transform.position.y + 0.5f);
        float distToWaypoint = Vector2.Distance(unitPosCenter, CurrentPath.vectorPath[currentWaypoint]);
        if (distToWaypoint < nextWaypointDistance)
        {
            Move(CurrentPath.vectorPath[currentWaypoint].GetGridPos());

            // Reset localPosition because localPosition is being offset when moving between tiles
            // Once unit has reached a tile then it should snap to that new tile - which is what this is doing
            transform.localPosition = Vector2.zero;

            if (currentWaypoint + 1 < CurrentPath.vectorPath.Count)
            {
                currentWaypoint++;
            }
            else
            {
                ReachedEndOfPath = true;
            }
        }

        Vector2 moveDir = (CurrentPath.vectorPath[currentWaypoint] - unitPosCenter).normalized;
        Vector2 velocity = moveDir * Speed;

        if (!ReachedEndOfPath)
        {
            transform.localPosition += (Vector3)velocity * Time.deltaTime;
        }
        else
        {
            ReachedEndOfPath = false;
            return true;
        }
        return false;
    }

    void Update()
    {
        bool reachedDestination = false;
        if (CurrentPath != null)
        {
            reachedDestination = MoveAlongPath();
        }
        if (reachedDestination)
        {
            CheckWorkableTask();
        }
    }
}