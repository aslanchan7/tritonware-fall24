using System.Collections.Generic;
using UnityEngine;


public abstract class Unit : Entity, IDamageable
{
    public override bool BlocksMovement => true;
    public override bool BlocksVision => false;

    public int Health = 100;
    public int MaxHealth = 100;
    public float Speed = 5f;
    public SpriteRenderer SelectIndicator;
    public UnitDisplay UnitDisplay;

    // Pathfinding should be done as a series of Move() calls
    public void Pathfind(Vector2Int targetPos)
    {
        // todo
    }

    // Finalizes moves a unit from one tile to another
    // In game this should normally be called for neighboring tiles only
    public void Move(Vector2Int targetPos)
    {
        if (targetPos == Pos) return;

        MapTile targetTile = MapManager.Instance.GetTile(targetPos);
        if (targetTile.ContainedUnit != null)
        {
            Debug.LogError("Tried to move into tile occupied by unit");
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
        foreach (MapTile tile in MapManager.Instance.GetTilesInRadius(Pos,radius))
        {
            if (tile.ContainedUnit != null)
            {
                result.Add(tile.ContainedUnit);
            }
        }
        return result;
    }
    
}