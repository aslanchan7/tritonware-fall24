using Unity.VisualScripting;
using UnityEngine;

public class GetInBedTask : Task
{
    public override bool CancelOnInterrupt => true;
    private HospitalBed bed;

    public override void AssignTask(Unit worker)
    {
        base.AssignTask(worker);
        bed = Structure as HospitalBed;
        if (bed == null) Debug.LogError("Error matching hospital task to bed");
        if (bed.Patient != null) bed.RemovePatient();
        bed.ReservePatient(worker);
    }

    public override void StartTask()
    {
        base.StartTask();
        bed.InsertPatient(Worker);

        FinishTask();
    }

    public override void RemoveTask()
    {
        bed.UnReservePatient();
        if (Worker != null) Worker.ClearPath();
        base.RemoveTask();
    }


}