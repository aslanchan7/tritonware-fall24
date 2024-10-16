using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public abstract class AlliedUnit : Unit
{
    public override Team Team => ForceEnemy ? Team.Enemy : Team.Allied;

    public Dictionary<Tasks, float> Efficiency;
    public float InfectionProgress = 0f;
    public float IncrementTimeInterval = 2f; // every 2 seconds increase InfectionProgress by 1%
    public float LastIncremented;
    [SerializeField] TextMeshPro infectionText;

    public bool ForceEnemy = false;     // forces this unit to be considered as an enemy for testing
    public bool IsControllable()
    {
        return true; // todo
    }
    public abstract void SetEfficiencyValues();

    public void GetInfected()
    {
        // this method gets called when an enemy attacks this unit

        // if not infected yet, then start infection
        if (InfectionProgress == 0f)
        {
            InfectionProgress = 0.01f;
        }
        // if already infected, speed it up by increasing infected rate by another 1%
        else
        {
            InfectionProgress += 0.01f;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        LastIncremented = Time.time;
    }

    protected override void Update()
    {
        base.Update();
        if (InfectionProgress != 0f && !(InfectionProgress >= 1f))
        {
            if (Time.time - LastIncremented >= IncrementTimeInterval)
            {
                InfectionProgress += 0.01f;
                LastIncremented = Time.time;
                infectionText.text = ((double)InfectionProgress).ToString();
            }
        }

        if (InfectionProgress >= 1f)
        {

        }
    }
}