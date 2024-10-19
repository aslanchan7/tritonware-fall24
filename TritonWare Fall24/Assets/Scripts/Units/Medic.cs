using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : AlliedUnit
{
    public override void SetEfficiencyValues()
    {
        Efficiency.Add(Tasks.Hospital, 1f);
        Efficiency.Add(Tasks.Lab, 0.7f);
    }
}
