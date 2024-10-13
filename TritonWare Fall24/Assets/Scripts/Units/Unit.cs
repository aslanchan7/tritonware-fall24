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
    public Vector2Int Destination;
    [HideInInspector] public bool ReachedEndOfPath;
    private int currentWaypoint = 0;
    // private float nextWaypointDistance = .2f;
    private bool enableMovement = true;
    private int routeReservePenalty = 200;
    private int standingPenalty = 10000;
    private int passingPenalty = 1000;
    private int interruptsUntilRepath = 4;
    private int currentInterrupts = 0;
    private int repathsUntilGiveUp = 3;
    private int currentRepaths = 0;
    private Queue<TileReservation> reservedTiles = new Queue<TileReservation>();

    protected Vector2Int advanceMoveDestination = PathfindingUtils.InvalidPos;
    private bool isReversing = false;


    public void Awake()
    {
        Seeker = GetComponent<Seeker>();
    }

    public void Place(Vector2Int pos)
    {
        Pos = pos;
        MapTile targetTile = MapManager.Instance.GetTile(pos);
        transform.SetParent(MapManager.Instance.GetTile(pos).transform, false);
        if (targetTile.ContainedUnit != null)
        {
            Debug.LogError("Tried to spawn into tile occupied by unit");
        }
        targetTile.ContainedUnit = this;
        ReserveTile(pos, standingPenalty);
        GameManager.AllUnits.Add(this);
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

    public IEnumerator PathfindCoroutine(Vector2Int targetPos, bool resetCounters = true)
    {
        if (resetCounters)
        {
            currentRepaths = 0;
            currentInterrupts = 0;
        }
        yield return StartCoroutine(Pathfind(targetPos));
    }

    // Pathfinding should be done as a series of Move() calls
    private IEnumerator Pathfind(Vector2Int targetPos)
    {
        while (reservedTiles.Count > 0) UnreserveTile();
        Vector2 startPos = Pos.GetTileCenter();
        Vector2 targetPosCenter = targetPos.GetTileCenter();
        Seeker.StartPath(startPos, targetPosCenter, OnPathComplete);
        Destination = targetPos;
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
        if (!targetTile.IsPassable() && targetTile.ContainedUnit != this)
        {
            Debug.LogWarning(this.gameObject.name + " tried to move into occupied tile");
        }
        if (reservedTiles.Count > 0)
        {
            UnreserveTile();
            if (reservedTiles.Count > 0)  ChangeNextTilePenalty(passingPenalty);
        }
        MapTile prevTile = MapManager.Instance.GetTile(Pos);
        prevTile.ContainedUnit = null;
        // PathfindingUtils.SetWalkable(Pos, prevTile.IsPassable());
        transform.SetParent(targetTile.transform, false);
        targetTile.ContainedUnit = this;
        Pos = targetPos;
        currentInterrupts = 0;
        currentRepaths = 0;
        // PathfindingUtils.SetWalkable(Pos, targetTile.IsPassable());
    }

    public void OnPathComplete(Path p)
    {
        ClearPath(false);
        if (!p.error)
        {
            CurrentPath = p;

            // currentWaypoint skips path's 1st tile because the 1st tile is the current tile that the unit is on
            currentWaypoint = 1;


            foreach (var waypoint in CurrentPath.vectorPath)
            {
                ReserveTile(waypoint.GetGridPos(), routeReservePenalty);
            }
        }
        else
        {
            Debug.LogError("Error while finding path using A*");
        }
    }

    public void Damage(int value)
    {
        Health -= value;
        UnitDisplay.UpdateDisplay();
        if (Health <= 0) TriggerDeath();
    }

    private void TriggerDeath()
    {
        GameManager.AllUnits.Remove(this);
        MapManager.Instance.GetTile(Pos).ContainedUnit = null;
        ClearPath();
        UnitController.Instance.SelectedUnits.Remove(this);
        Destroy(this.gameObject);

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

    // pauses movement towards destination for set amount of time. If time is negative, stop and also destroys the path.
    private void InterruptMove(float time, bool preservePos)
    {
        if (!preservePos)
        {
            transform.localPosition = Vector2.zero;
        }
        if (time < 0)
        {
            ClearPath();
        }
        else
        {
            StopCoroutine("PauseMove");
            StartCoroutine(PauseMove(time));
        }

    }

    protected void ClearPath(bool stay = true)
    {
        while (reservedTiles.Count > 0)
        {
            UnreserveTile();
        }
        CurrentPath = null;
        if (stay) ReserveTile(Pos, standingPenalty);
    }

    private void ReserveTile(Vector2Int pos, int penalty)
    {
        reservedTiles.Enqueue(new TileReservation(pos, penalty));
        PathfindingUtils.ChangePenalty(pos, penalty);
    }
    private void ChangeNextTilePenalty(int penalty)
    {
        TileReservation nextTile = reservedTiles.Peek();
        nextTile.Penalty += penalty;
        PathfindingUtils.ChangePenalty(nextTile.Pos, penalty);
    }

    private void UnreserveTile()
    {
        TileReservation tr = reservedTiles.Dequeue();
        PathfindingUtils.ChangePenalty(tr.Pos, -tr.Penalty);
    } 

    private IEnumerator PauseMove(float time)
    {
        enableMovement = false;
        yield return new WaitForSeconds(time);
        enableMovement = true;
    }

    protected virtual void FinishPath()
    {
        ClearPath();
        
    }


    // returns true if destination is reached
    public bool MoveAlongPath()
    {
        if (CurrentPath == null) return false;

        ReachedEndOfPath = false;

        Vector2Int nextTargetPos = CurrentPath.vectorPath[currentWaypoint].GetGridPos();
        if (currentRepaths >= repathsUntilGiveUp)
        {
            Debug.LogWarning("Giving up -- terminating path");
            currentInterrupts = 0;
            currentRepaths = 0;
            InterruptMove(-1, false);
            return true;
        }
        if (currentInterrupts >= interruptsUntilRepath)
        {
            Debug.Log("Retrying -- Finding new path");
            currentInterrupts = 0;
            currentRepaths++;
            InterruptMove(-1, true);
            StartCoroutine(PathfindCoroutine(Destination, false));
            return false;
        }
        if (!MapManager.Instance.IsPassable(nextTargetPos) && MapManager.Instance.GetTile(nextTargetPos).ContainedUnit != this)
        {
            //Debug.Log("Path blocked -- waiting, current interrupts: " + currentInterrupts);
            currentInterrupts++;
            InterruptMove(0.05f, true);
            return false;
        }



        // Find the center position of the unit
        Vector3 unitPosCenter = new(transform.position.x + 0.5f, transform.position.y + 0.5f);
        // float distToWaypoint = Vector2.Distance(unitPosCenter, CurrentPath.vectorPath[currentWaypoint]);
        



        // check if local position has deviated from grid position by one tile
        if (Mathf.Abs(transform.localPosition.x) > 0.99 || Mathf.Abs(transform.localPosition.y) > 0.99)
        {


            Move(nextTargetPos);

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
            FinishPath();
            return true;
        }
        return false;
    }

    // Called every frame; try to move into a tile regardless of whether it is occupied or not, 
    // hoping that it will become unoccupied when the unit arrives. If tile is still occupied,
    // go back to original position at a slower speed.
    protected void AdvanceMove()
    {
        if (CurrentPath != null || advanceMoveDestination == PathfindingUtils.InvalidPos) return;

        if (!isReversing && (Mathf.Abs(transform.localPosition.x) > 1 || Mathf.Abs(transform.localPosition.y) > 1))
        {
            if (MapManager.Instance.IsPassable(advanceMoveDestination))
            {
                Move(advanceMoveDestination);
                advanceMoveDestination = PathfindingUtils.InvalidPos;
                transform.localPosition = Vector2.zero;
                AdvanceMoveSucceed();
                AdvanceMoveEnd();
                return;
            }
            else
            {
                isReversing = true;
                AdvanceMoveFail();
            }
        }

        Vector2 unitPosCenter = new(transform.position.x + 0.5f, transform.position.y + 0.5f);
        Vector2 moveDir, velocity;
        if (!isReversing)
        {
            moveDir = (advanceMoveDestination.GetTileCenter() - unitPosCenter).normalized;
            velocity = moveDir * Speed * 1.2f;
        }
        else
        {
            moveDir = (Pos.GetTileCenter() - unitPosCenter).normalized;
            velocity = moveDir * Speed * 0.3f;
        }
        transform.localPosition += (Vector3)velocity * Time.deltaTime;
        if (isReversing && transform.localPosition.magnitude <= 0.05f)
        {
            isReversing = false;
            transform.localPosition = Vector2.zero;
            advanceMoveDestination = PathfindingUtils.InvalidPos;
            AdvanceMoveEnd();
        }

    }

    protected virtual void AdvanceMoveSucceed() { }
    protected virtual void AdvanceMoveFail() { }
    protected virtual void AdvanceMoveEnd() { }

    protected float DistanceToDestination()
    {
        if (CurrentPath == null) return -1;
        return Vector2Int.Distance(Pos, Destination);
    }


    public Vector2Int GetPendingWaypoint(int index)
    {
        try
        {
            return CurrentPath.vectorPath[currentWaypoint + index].GetGridPos();
        }
        catch (System.Exception)        // either no path or index out of range
        {
            return PathfindingUtils.InvalidPos;
        }
    }

    protected virtual void Update()
    {
        bool reachedDestination = false;
        if (enableMovement && CurrentPath != null)
        {
            reachedDestination = MoveAlongPath();
        }
        if (reachedDestination)
        {
            CheckWorkableTask();
        }
        if (CurrentPath == null && advanceMoveDestination != PathfindingUtils.InvalidPos)
        {
            AdvanceMove();
        }
    }
}