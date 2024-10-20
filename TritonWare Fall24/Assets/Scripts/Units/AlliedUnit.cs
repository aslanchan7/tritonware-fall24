using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections;

public abstract class AlliedUnit : Unit
{
    public override Team Team => ForceEnemy ? Team.Enemy : Team.Allied;
    private float regenSpeed = 1f;  // passive hp regen per second
    private float regenTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        Efficiency = new();
        SetEfficiencyValues();
    }

    public Dictionary<Tasks, float> Efficiency;
    public bool ForceEnemy = false;     // forces this unit to be considered as an enemy for testing
    public bool IsControllable()
    {
        return true; // todo
    }

    protected override void Update()
    {
        base.Update();
        if (regenTimer <= 0f)
        {
            Heal(1);
            regenTimer = 1 / regenSpeed;
        }
        else
        {
            regenTimer -= Time.deltaTime;
        }
    }

    public abstract void SetEfficiencyValues();
}