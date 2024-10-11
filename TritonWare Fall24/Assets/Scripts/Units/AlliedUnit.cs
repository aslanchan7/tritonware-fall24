using System;
using System.Collections.Generic;

public abstract class AlliedUnit : Unit
{
    public override Team Team => ForceEnemy ? Team.Enemy : Team.Allied;

    public Dictionary<Tasks, float> Efficiency;

    public bool ForceEnemy = false;     // forces this unit to be considered as an enemy for testing
    public bool IsControllable()
    {
        return true; // todo
    }
    public abstract void SetEfficiencyValues();
}