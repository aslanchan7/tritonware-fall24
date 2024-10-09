using UnityEngine;

public class MakeCureTask : Task
{
    public override bool CancelOnInterrupt => false;

    public override void WorkTask()
    {
        base.WorkTask();
        Debug.Log("Working");
    }
}