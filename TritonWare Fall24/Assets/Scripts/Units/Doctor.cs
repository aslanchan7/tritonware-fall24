using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : AlliedUnit
{
    public FieldOfView fieldOfView;
    public override void SetEfficiencyValues()
    {
        // Doctor can do everything slightly faster
        Efficiency.Add(Tasks.Hospital, 1.1f);
        Efficiency.Add(Tasks.Lab, 1.1f);
    }

    protected override void Update()
    {
        // This section is from base.Update() but excludes the infection as the Doctor can't get infected
        /////////////////////////////////////
        bool reachedDestination = false;
        if (enableMovement && CurrentPath != null)
        {
            reachedDestination = MoveAlongPath();
        }
        if (reachedDestination)
        {
            CheckWorkableTask();
        }
        if (CurrentPath == null && advanceMoveDestination != PathfindingUtils.InvalidPos)
        {
            AdvanceMove();
        }
        /////////////////////////////////////

        fieldOfView.SetOrigin(transform.position);
    }
}
