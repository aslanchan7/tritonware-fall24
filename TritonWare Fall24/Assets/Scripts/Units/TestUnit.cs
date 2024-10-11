using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : AlliedUnit
{
    public override void SetEfficiencyValues()
    {
        Efficiency.Add(Tasks.Lab, 1f);
        Efficiency.Add(Tasks.Hospital, 1f);
    }
}
