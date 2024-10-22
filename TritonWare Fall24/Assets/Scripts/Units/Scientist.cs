using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist : AlliedUnit
{
    public override UnitType GetUnitType()
    {
        return UnitType.Scientist;
    }

    public override void SetEfficiencyValues()
    {
        Efficiency.Add(Tasks.Hospital, 0.7f);
        Efficiency.Add(Tasks.Lab, 1f);
    }
}
