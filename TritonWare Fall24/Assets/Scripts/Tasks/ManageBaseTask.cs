using UnityEngine;

public class ManageBaseTask : Task
{
    public override bool CancelOnInterrupt => true;




    public override void StartTask()
    {
        base.StartTask();
        if (Worker is Doctor)
            VisionController.Instance.ToggleVision(true);
    }
    public override void RemoveTask()
    {
        VisionController.Instance.ToggleVision(false);
        base.RemoveTask();
    }

    public override void WorkTask()
    {
        base.WorkTask();
    }




}