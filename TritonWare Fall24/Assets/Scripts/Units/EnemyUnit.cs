
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyState
{
    Idle, AttackClosest, Rush
}

// Enemy unit that uses melee attacks
public abstract class EnemyUnit : Unit
{
    public override Team Team => Team.Enemy;
    public int AttackDamage = 10;

    private float farTargetDistance = 8f;
    private float closeRetargetInterval = 0.5f; // how often to reselect a target in seconds
    private float farRetargetInterval = 2f; // how often to reselect a target in seconds
    private float currentRetargetTimer = 0;
    private IDamageable currentAttackTarget;
    public EnemyState CurrentState = EnemyState.Idle;

    private bool isAttacking = false;
    protected float attackCooldown = 0.5f;
    private float attackTimer = 0f;

    private float loiterInterval = 3f;
    private float loiterTimer = 0f;


    protected override void Update()
    {
        base.Update();

        if (CurrentState == EnemyState.Idle)
        {
            if (GetAttackableTarget(12f) != null)
            {
                ChangeState(EnemyState.AttackClosest);
                return;
            }

            if (CurrentPath == null && loiterTimer > 0f) loiterTimer -= Time.deltaTime;
            else
            {
                var freeTiles = FreeTilesInRadius(8f);
                if (freeTiles.Count == 0) return;
                Vector2Int loiterDest = freeTiles[Random.Range(0, freeTiles.Count)];
                if (CurrentPath == null)
                {
                    StartCoroutine(PathfindCoroutine(loiterDest));
                    loiterTimer = loiterInterval * Random.Range(0.5f, 2f);
                }
            }
            return;
        }

        if (currentRetargetTimer > 0)
        {
            currentRetargetTimer -= Time.deltaTime;
        }
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        if (!isAttacking && CanAttack())
        {
            IDamageable attackableTarget = GetAttackableTarget();
            if (attackableTarget != null)                                    // close enough for attack, stop pathing
            {
                currentAttackTarget = attackableTarget;
                ClearPath();
                TryAttack();
            }
            else if (currentAttackTarget != null)
            {

                if (
                    CurrentPath != null &&                                                                    // is currently pathfinding
                    DistanceToDestination() < 3 &&                                                            // almost reached the current destination
                    Vector2Int.Distance(Destination, FindAttackablePosition(currentAttackTarget)) > 2         // the current destination is not close enough to target
                )
                {
                    // update the destination to be closer to target
                    // Debug.Log("close retarget");
                    UpdateTarget();
                    return;
                }

            }
            if (currentRetargetTimer <= 0)
            {
                UpdateTarget();
                return;
            }
        }
    }

    public void ChangeState(EnemyState state)
    {
        CurrentState = state;
        ClearPath();
    }

    protected override void AdvanceMoveFail()
    {
        base.AdvanceMoveFail();
        LandAttack();
    }

    protected override void AdvanceMoveSucceed()
    {
        base.AdvanceMoveSucceed();
        advanceMoveDestination = currentAttackTarget.Pos;
    }

    protected override void AdvanceMoveEnd()
    {
        base.AdvanceMoveEnd();
        isAttacking = false;
    }

    private void TryAttack()
    {
        advanceMoveDestination = currentAttackTarget.Pos;
        isAttacking = true;
    }

    protected virtual void LandAttack()
    {
        attackTimer = attackCooldown;
        currentAttackTarget.Damage(AttackDamage);
    }


    private void UpdateTarget()
    {
        if (currentAttackTarget != null && Vector2Int.Distance(Pos, currentAttackTarget.Pos) < farTargetDistance)
            currentRetargetTimer = closeRetargetInterval;
        else
            currentRetargetTimer = farRetargetInterval;

        // gets the target depending on definition in subclass, then tries to pathfind to a position where
        // it can stand next to the target

        IDamageable newTarget = FindAttackTarget();
        if (newTarget != null)
        {
            currentAttackTarget = newTarget;
            //Debug.Log($"{name} Targeting : {newTarget.name}");

            // if more than 1 tile away from target, pathfind to a position where it can attack
            if (Vector2Int.Distance(Pos, newTarget.Pos) > 1.5)
            {
                Vector2Int attackPos = FindAttackablePosition(newTarget);

                if (attackPos != PathfindingUtils.InvalidPos)
                {
                    //Debug.Log($"Position to attack : {attackPos}");
                    StartCoroutine(PathfindCoroutine(attackPos));
                }
            }
        }
    }

    // finds a valid tile to stand to attack the target, prioritizing the closest
    private Vector2Int FindAttackablePosition(IDamageable target)
    {
        Vector2Int closest = PathfindingUtils.InvalidPos;
        float closestDist = float.MaxValue;
        foreach (Vector2Int occupiedPos in ((Entity)target).GetOccupiedPositions())
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



    // On reaching destination, find a new target
    protected override void FinishPath()
    {
        base.FinishPath();
        if (CurrentState != EnemyState.Idle)
            UpdateTarget();
    }

    public bool CanAttack()
    {
        return attackTimer <= 0;
    }

    // closest target within attacking distance right now
    protected abstract IDamageable GetAttackableTarget(float radius = 1.5f);

    protected abstract IDamageable FindAttackTarget();
}