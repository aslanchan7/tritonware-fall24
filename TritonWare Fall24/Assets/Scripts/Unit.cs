using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Collections;


public abstract class Unit : Entity, IDamageable
{
    public override bool BlocksMovement => true;
    public override bool BlocksVision => false;
    public override Vector2Int Size => new Vector2Int(1, 1);

    public int Health = 100;
    public int MaxHealth = 100;
    public float Speed = 5f;
    public SpriteRenderer SelectIndicator;
    public UnitDisplay UnitDisplay;

    // Pathfinding
    public Seeker Seeker;
    public Path CurrentPath;
    public bool ReachedEndOfPath;
    private int currentWaypoint = 0;
    private float nextWaypointDistance = .2f;

    // public Queue<Vector2> Moves = new();
    // public Vector2 NextWaypoint;
    // private Vector2 moveDir = Vector2.zero;

    public void Awake()
    {
        Seeker = GetComponent<Seeker>();
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

    // Finalizes moves a unit from one tile to another
    // In game this should normally be called for neighboring tiles only
    public void Move(Vector2Int targetPos)
    {
        // if (targetPos == Pos) return;

        // MapTile targetTile = MapManager.Instance.GetTile(targetPos);
        // if (!targetTile.IsPassable())
        // {
        //     Debug.LogError("Tried to move into occupied tile");
        // }
        // MapManager.Instance.GetTile(Pos).ContainedUnit = null;
        // transform.SetParent(MapManager.Instance.GetTile(targetPos).transform, false);
        // targetTile.ContainedUnit = this;
        // Pos = targetPos;
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

    void Update()
    {
        // If there is no path then don't run any of this code
        // Note: If anything is to be added in Update func do it above these lines of code
        if (CurrentPath == null) return;

        ReachedEndOfPath = false;
        // Find the center position of the unit
        Vector3 unitPosCenter = new(transform.position.x + 0.5f, transform.position.y + 0.5f);
        float distToWaypoint = Vector2.Distance(unitPosCenter, CurrentPath.vectorPath[currentWaypoint]);
        if (distToWaypoint < nextWaypointDistance)
        {
            MapTile targetTile = MapManager.Instance.GetTile(CurrentPath.vectorPath[currentWaypoint].GetGridPos());
            MapManager.Instance.GetTile(Pos).ContainedUnit = null;
            transform.SetParent(targetTile.transform, false);
            targetTile.ContainedUnit = this;
            Pos = CurrentPath.vectorPath[currentWaypoint].GetGridPos();

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
    }
}