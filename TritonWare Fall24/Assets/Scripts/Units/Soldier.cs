using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : AlliedUnit
{
    public override UnitType GetUnitType()
    {
        return UnitType.Solider;
    }

    public override void SetEfficiencyValues()
    {
        Efficiency.Add(Tasks.Hospital, 0.7f);
        Efficiency.Add(Tasks.Lab, 0.7f);
    }
}
