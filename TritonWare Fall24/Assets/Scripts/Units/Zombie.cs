using UnityEngine;

public class Zombie : EnemyUnit
{
    // attacks the closest allied or visitor unit
    protected override Entity FindAttackTarget()
    {
        Unit closest = null;
        float closestDist = float.MaxValue;
        foreach (Unit unit in GameManager.GetUnitsOfTeam(Team.Allied))
        {
            float dist = Vector2Int.Distance(unit.Pos, Pos);
            if (closest == null || dist < closestDist)
            {
                closest = unit;
                closestDist = dist;
            } 
        }
        foreach (Unit unit in GameManager.GetUnitsOfTeam(Team.Visitor))
        {
            float dist = Vector2Int.Distance(unit.Pos, Pos);
            if (closest == null || dist < closestDist)
            {
                closest = unit;
                closestDist = dist;
            }
        }
        return closest;
    }
}