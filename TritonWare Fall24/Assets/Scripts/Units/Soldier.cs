using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : AlliedUnit
{
    public override void SetEfficiencyValues()
    {
        Efficiency.Add(Tasks.Hospital, 0.5f);
        Efficiency.Add(Tasks.Lab, 0.5f);
    }
}
