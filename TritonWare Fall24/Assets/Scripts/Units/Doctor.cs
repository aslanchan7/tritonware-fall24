using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : AlliedUnit
{
    public FieldOfView fieldOfView;
    public override void SetEfficiencyValues()
    {
        // Doctor can do everything slightly faster
        Efficiency.Add(Tasks.Hospital, 1.3f);
        Efficiency.Add(Tasks.Lab, 1.3f);
    }

    protected override void Update()
    {
        base.Update();
    }
}
