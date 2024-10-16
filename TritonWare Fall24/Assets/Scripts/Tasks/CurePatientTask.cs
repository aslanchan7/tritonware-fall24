using UnityEngine;

public class CurePatientTask : Task
{
    public override bool CancelOnInterrupt => false;

    public override void WorkTask()
    {
        base.WorkTask();
        Debug.Log("Working");
    }
}