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

    public void Awake()
    {
        Seeker = GetComponent<Seeker>();
    }

    // Pathfinding should be done as a series of Move() calls
    public void Pathfind(Vector2Int targetPos)
    {
        Vector2 startPos = Pos.GetTileCenter();
        Vector2 targetPosCenter = targetPos.GetTileCenter();
        Seeker.StartPath(startPos, targetPosCenter, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            CurrentPath = p;

            for (int i = 0; i < CurrentPath.vectorPath.Count; i++)
            {
                Vector2Int targetPos = new Vector2Int((int)CurrentPath.vectorPath[i].x, (int)CurrentPath.vectorPath[i].y);
                Move(targetPos);
            }
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
        if (targetPos == Pos) return;

        MapTile targetTile = MapManager.Instance.GetTile(targetPos);
        if (!targetTile.IsPassable())
        {
            Debug.LogError("Tried to move into occupied tile");
        }
        MapManager.Instance.GetTile(Pos).ContainedUnit = null;
        transform.SetParent(MapManager.Instance.GetTile(targetPos).transform, false);
        targetTile.ContainedUnit = this;
        Pos = targetPos;
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
}