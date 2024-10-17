using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections;

public abstract class AlliedUnit : Unit
{
    public override Team Team => ForceEnemy ? Team.Enemy : Team.Allied;

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
    public abstract void SetEfficiencyValues();
}