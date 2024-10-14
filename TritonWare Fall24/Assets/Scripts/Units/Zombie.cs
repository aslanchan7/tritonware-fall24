using System.Collections.Generic;
using UnityEngine;

public class Zombie : EnemyUnit
{
    // attacks the closest allied or visitor unit
    protected override IDamageable FindAttackTarget()
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

    // 1.5 = 1 tile distance + diagonals
    protected override IDamageable GetAttackableTarget(float radius = 1.5f)
    {
        List<Unit> possible = new();
        foreach (Unit u in GetUnitsInRadius(radius))
        {
            if (GameManager.OpposingTeams(u.Team, Team)) possible.Add(u);

        }
        if (possible.Count > 0) { return possible[Random.Range(0, possible.Count)]; }
        return null;
    }
}