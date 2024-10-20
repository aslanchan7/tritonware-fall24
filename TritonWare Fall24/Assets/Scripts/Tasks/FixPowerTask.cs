using UnityEngine;

public class FixPowerTask : Task
{
    public override bool CancelOnInterrupt => true;

    private float repairProgress = 0f;
    private float repairSpeed = 0.2f;



    public override void StartTask()
    {
        base.StartTask();
        //
        repairProgress = 0f;
        
    }

    public override void FinishTask()
    {
        ((Workstation)Structure).ToggleEnabled(false, false);
        BlackoutController.Instance.FixBlackout();
        base.FinishTask();
    }


    public override void RemoveTask()
    {
        base.RemoveTask();
    }

    public override void WorkTask()
    {
        base.WorkTask();
        if (Worker is Doctor)
        {
            repairProgress += Time.deltaTime * repairSpeed;
        }
        
        if (repairProgress >= 1f)
        {
            FinishTask();
        }
    }

    public override float GetVisualProgress()
    {
        return repairProgress;
    }




}