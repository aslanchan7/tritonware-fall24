using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public Unit WeaponHolder;
    public int Damage = 10;
    public float Firerate = 2;  // shots per second
    public float WeaponRange = 6;   // radius in tiles
    public float Spread = 5;    // max angle deviation from aim (in degrees)
    private IDamageable currentTarget = null;
    private float currentWeaponCooldown = 0;
    private float targetSearchInterval = 0.1f;
    private float targetSearchTime = 0;  // interval to search for new target
    private bool shootingEnabled = false;
    private LayerMask layermask;
    private BulletTrail bulletTrail;
    public BulletTrail TrailPrefab;

    private void Awake()
    {
        // automatically sets parent transfom as weapon holder on startup
        Unit attachedUnit = GetComponentInParent<Unit>();
        if (attachedUnit != null)
        {
            WeaponHolder = attachedUnit;
        }
        else
        {
            Debug.LogWarning("Weapon is not attached to an object of type Unit");
        }
        bulletTrail = GetComponent<BulletTrail>();


        layermask = LayerMask.GetMask("Units", "Structures"); // anything that may interact with the path of a bullet
        shootingEnabled = true;
    }

    private void Update()
    {
        if (currentWeaponCooldown > 0)
        {
            currentWeaponCooldown -= Time.deltaTime;
        }
        if (shootingEnabled)
        {
            // target finding
            if (targetSearchTime <= 0)
            {
                // while shooting is enabled, search for the best target every 0.1s
                SelectTarget();
                targetSearchTime = targetSearchInterval;
            }
            else
            {
                targetSearchTime -= Time.deltaTime;
            }

            // shooting
            if (currentWeaponCooldown <= 0 && currentTarget != null)
            {
                FireAt(currentTarget);
                currentWeaponCooldown = 1 / Firerate;
            }
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
        Vector2 source = WeaponHolder.transform.position.GetTileCenter();
        foreach (IDamageable target in targets)
        {
            if (!GameManager.OpposingTeams(WeaponHolder.Team, target.Team))
            {
                // immediately reject teammates as potential target
                continue;
            }
            Vector2 direction = target.GetGameObject().transform.position.GetTileCenter() - WeaponHolder.transform.position.GetTileCenter();
            /*
            Instantiate(TrailPrefab, transform).RenderTrail
                (source, source + direction, 0.1f);
            */
            // cast a ray to each potential target in range to see if there is a valid path to it
            RaycastHit2D[] hits = Physics2D.RaycastAll(source, direction, WeaponRange, layermask);
            foreach (RaycastHit2D hit in hits)
            {
                Entity hitEntity = hit.collider.GetComponent<Entity>();
                if (hitEntity is IDamageable d && target == d)
                {
                    // found valid path to target
                    // Debug.DrawLine(source, hit.point, Color.white, 0.5f);
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
            }

        }
        return result;
    }

    // find the closest one or priority target
    private void SelectTarget()
    {
        List<IDamageable> targets = GetShootableTargets();
        if (targets.Count == 0)
        {
            currentTarget = null;
            return;
        }
        IDamageable closest = null;
        float minDist = float.MaxValue;
        foreach (IDamageable target in targets)
        {
            float dist = Vector2Int.Distance(target.Pos, WeaponHolder.Pos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = target;
            }
        }
        currentTarget = closest;
    }

    // Can only shoot at anything damageable
    private void FireAt(IDamageable target)
    {
        if (target == null || target.GetGameObject() == null) return;
        Vector2 direction = target.GetGameObject().transform.position.GetTileCenter() - WeaponHolder.transform.position.GetTileCenter();
        // Weapon Range +1 to account for target moving just out of range when firing
        float angleDeviation = UnityEngine.Random.Range(-Spread, Spread);   // inaccuracy
        direction = direction.Rotate(angleDeviation);
        RaycastHit2D[] hits = Physics2D.RaycastAll(WeaponHolder.transform.position.GetTileCenter(), direction, WeaponRange + 1, layermask);
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
        }
        // didn't hit anything
        ProjectileHit(null, WeaponHolder.transform.position.GetTileCenter() + direction);
    }

    // if target is null, shoot a trail to max range towards position
    // otherwise, shoot a trail to target at specified position
    private void ProjectileHit(Entity target, Vector2 position)
    {
        Vector2 origin = WeaponHolder.transform.position.GetTileCenter();
        Vector2 direction = (position - origin).normalized;
        float distance = Vector2.Distance(origin, position);
        float trailTravelDistance;
        if (target == null)
        {
            trailTravelDistance = (WeaponRange + 1);
        }
        else
        {
            trailTravelDistance = distance + 0.5f;
            if (target is IDamageable dam)
            {
                if (!GameManager.OpposingTeams(target.Team, WeaponHolder.Team))
                    Debug.LogWarning($"undefined weapon behavior - hit {target.name}");
                dam.Damage(Damage);
            }
        }

        bulletTrail.RenderProjectile(origin, origin + direction * trailTravelDistance);
        //Instantiate(TrailPrefab, transform).RenderTrail(origin, origin + direction * trailTravelDistance, 0.05f);
    }
}