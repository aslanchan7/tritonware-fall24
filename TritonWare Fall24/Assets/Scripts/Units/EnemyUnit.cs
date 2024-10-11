using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyUnit : Unit
{
    public override Team Team => Team.Enemy;

    private float farTargetDistance = 8f;
    private float closeRetargetInterval = 0.1f; // how often to reselect a target in seconds
    private float farRetargetInterval = 2f; // how often to reselect a target in seconds
    private float currentRetargetTimer = 0;
    private Entity currentAttackTarget;
    protected abstract Entity FindAttackTarget();


    protected override void Update()
    {
        base.Update();
        if (currentRetargetTimer > 0)
        {
            currentRetargetTimer -= Time.deltaTime;
        }
        if (currentRetargetTimer <= 0)
        {
            if (currentAttackTarget != null && Vector2Int.Distance(Pos, currentAttackTarget.Pos) < farTargetDistance)
                currentRetargetTimer = closeRetargetInterval;
            else 
                currentRetargetTimer = farRetargetInterval;

            // gets the target depending on definition in subclass, then tries to pathfind to a position where
            // it can stand next to the target

            Entity newTarget = FindAttackTarget();
            if (newTarget != null)
            {
                currentAttackTarget = newTarget;
                //Debug.Log($"{name} Targeting : {newTarget.name}");
                if (Vector2Int.Distance(Pos, newTarget.Pos) > 1.5)
                {
                    Vector2Int attackPos = FindAttackablePosition(newTarget);
                    if (attackPos != new Vector2Int(int.MinValue, int.MinValue))
                    {
                        //Debug.Log($"Position to attack : {attackPos}");
                        StartCoroutine(PathfindCoroutine(attackPos));
                    }
                }
            }
        }


    }
    
    // finds a valid tile to stand to attack the target, prioritizing the closest
    private Vector2Int FindAttackablePosition(Entity target)
    {
        Vector2Int closest = new Vector2Int(int.MinValue, int.MinValue);
        float closestDist = float.MaxValue;
        foreach (Vector2Int occupiedPos in target.GetOccupiedPositions())
        {
            foreach (Vector2Int freeNeighbor in MapManager.Instance.GetFreeNeighbors(occupiedPos))
            {
                float dist = Vector2Int.Distance(freeNeighbor, Pos);
                if (dist < closestDist)
                {
                    closest = freeNeighbor;
                    closestDist = dist;
                }
            } 
        }
        return closest;
    }


}