using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Unit WeaponHolder;
    public int Damage = 10;
    public float Firerate = 2;
    private float currentWeaponCooldown = 0;
    private bool shootingEnabled = false;
    public float WeaponRange;
    private LayerMask layermask = LayerMask.GetMask("Units", "Structures"); // anything that may interact with the path of a bullet

    private void Update()
    {
        if (shootingEnabled && currentWeaponCooldown > 0)
        {
            currentWeaponCooldown -= Time.deltaTime;
        }
    }

    public void ToggleShooting(bool toggle)
    {
        shootingEnabled = toggle;
    }

    // Rule of projectile blocking: only things that block vision and enemy entities block bullets
    // anything that does not block movement do not interact with bullets
    // other "small" structures that do not block vision do not block bullets
    // allied units do not block bullets
    private List<IDamageable> GetShootableTargets()
    {
        
        List<IDamageable> targets = WeaponHolder.GetUnitsInRadius(WeaponRange).Cast<IDamageable>().ToList();
        // TODO: find other damageables (if any)
        List<IDamageable> result = new List<IDamageable>();
        foreach (IDamageable target in targets)
        {
            Vector2 direction = target.Pos - WeaponHolder.Pos; 
            // cast a ray to each potential target in range to see if there is a valid path to it
            RaycastHit2D[] hits = Physics2D.RaycastAll(WeaponHolder.Pos, direction, WeaponRange, layermask);
            foreach (RaycastHit2D hit in hits)
            {
                Entity hitEntity = hit.collider.GetComponent<Entity>();
                if (hitEntity is IDamageable d && target == d)
                {
                    // found valid path to target
                    result.Add(target);
                    break;
                }
                else if (hitEntity.BlocksVision)
                {
                    // stop when hitting a wall
                    break;
                }
                else if (GameManager.OpposingTeams(hitEntity.Team, WeaponHolder.Team))
                {
                    // no valid path to target as an enemy is blocking
                    break;
                }
                else if (!hitEntity.BlocksMovement)
                {
                    // phase through anything that doesn't block movement (e.g. open doors)
                    continue;
                }
                else if (!GameManager.OpposingTeams(hitEntity.Team, WeaponHolder.Team))
                {
                    // able to target through teammates
                    continue;
                }
                else
                {
                    Debug.LogWarning("undefined raycast behavior");
                    break;
                }

            }
            return result;
        }
        return null;
    }
    
    // find the closest one or priority target

    // Can only shoot at anything damageable
    private void FireAt(IDamageable target)
    {
        Vector2 direction = target.Pos - WeaponHolder.Pos;  // TODO + inaccuracy
        RaycastHit2D[] hits = Physics2D.RaycastAll(WeaponHolder.Pos, direction, WeaponRange, layermask);
        foreach (RaycastHit2D hit in hits)
        {
            Entity hitEntity = hit.collider.GetComponent<Entity>();
            if (hitEntity.BlocksVision)
            {
                // stop when hitting a wall
                ProjectileHit(hitEntity, hit.point);
                return;
            }
            else if (GameManager.OpposingTeams(hitEntity.Team, WeaponHolder.Team))
            {
                // hit an enemy (intended or not)
                ProjectileHit(hitEntity, hit.point);
                return;
            }
            else if (!hitEntity.BlocksMovement)
            {
                // phase through anything that doesn't block movement (e.g. open doors)
                continue;
            }
            else if (!GameManager.OpposingTeams(hitEntity.Team, WeaponHolder.Team))
            {
                // able to shoot through teammates
                continue;
            }
            else
            {
                Debug.LogWarning("undefined raycast behavior");
                ProjectileHit(hitEntity, hit.point);
                return;
            }
        }
        // didn't hit anything
        ProjectileHit(null, WeaponHolder.Pos + direction);
    }

    private void ProjectileHit(Entity target, Vector2 position)
    {
        if (target != null)
        {
            // render a trail to infinity
        }

        if (target is IDamageable dam)
        {
            if (GameManager.OpposingTeams(target.Team, WeaponHolder.Team))
                Debug.LogWarning("undefined weapon behavior");
            dam.Damage(Damage);
        }
        // render a trail to hit point
    }



}