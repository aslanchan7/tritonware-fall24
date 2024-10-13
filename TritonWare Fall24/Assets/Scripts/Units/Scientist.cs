using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist : AlliedUnit
{
    public override void SetEfficiencyValues()
    {
        Efficiency.Add(Tasks.Hospital, 0.5f);
        Efficiency.Add(Tasks.Lab, 1f);
    }
}
