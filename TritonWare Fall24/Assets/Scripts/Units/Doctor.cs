using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class Doctor : AlliedUnit
{
    public FieldOfView FieldOfView;
    private bool lowHealth = false;


    protected override void Awake()
    {
        base.Awake();
        FieldOfView = GetComponent<FieldOfView>();
    }

    public override void SetEfficiencyValues()
    {
        // Doctor can do everything slightly faster
        Efficiency.Add(Tasks.Hospital, 1.3f);
        Efficiency.Add(Tasks.Lab, 1.3f);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Damage(int value)
    {
        base.Damage(value);
        if (!lowHealth && Health <= MaxHealth / 3)
        {
            lowHealth = true;
            OverlayManager.Instance.CreateTargetIndicator(Pos, TargetIndicatorType.LowHealth);
        }
    }

    public override void Heal(int value)
    {
        base.Heal(value);
        if (lowHealth && Health >= MaxHealth / 3 + 10)
        {
            lowHealth = false;
        }
    }

    protected override void TriggerDeath()
    {
        GameManager.Instance.GameOver();
        base.TriggerDeath();
    }

    public bool CanSee(Unit unit)
    {
        if (unit == this) return true;
        if (Vector2.Distance(unit.Pos, Pos) > FieldOfView.viewRadius + 1) return false;


        Vector2 direction = unit.transform.position.GetTileCenter() - transform.position.GetTileCenter();
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position.GetTileCenter(), direction, FieldOfView.viewRadius + 1, LayerMask.GetMask("Units", "Structures"));
        foreach (RaycastHit2D hit in hits)
        {
            Entity hitEntity = hit.collider.GetComponent<Entity>();
            if (unit == hitEntity)
            {

                return true;
            }
            else if (hitEntity.BlocksVision)
            {
                return false;
            }
        }
        return false;
    }

    public override UnitType GetUnitType()
    {
        return UnitType.Doctor;
    }
}
