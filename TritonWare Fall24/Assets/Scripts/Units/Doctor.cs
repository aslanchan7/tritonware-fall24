using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : AlliedUnit
{
    public override void SetEfficiencyValues()
    {
        // Doctor can do everything slightly faster
        Efficiency.Add(Tasks.Hospital, 1.1f);
        Efficiency.Add(Tasks.Lab, 1.1f);
    }
}
