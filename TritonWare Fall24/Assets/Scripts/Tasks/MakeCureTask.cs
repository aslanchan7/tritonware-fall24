using UnityEngine;

public class MakeCureTask : Task
{
    public override bool CancelOnInterrupt => false;


    private float labProgress = 0f;
    private float labSpeed = 0.1f;     // fraction of progress per second
    private float attemptStartCooldown = 0f;
    private bool cureInProgress = false;

    public override void StartTask()
    {
        base.StartTask();
    }

    public override void WorkTask()
    {
        ResourceManager supplies = GameManager.Instance.SupplyResource;
        base.WorkTask();

        if (cureInProgress)
        {
            labProgress += labSpeed * ((AlliedUnit)Worker).Efficiency[Tasks.Lab] * Time.deltaTime;
            if (labProgress >= 1)
            {
                FinishCure();
            }
        }

        else if (attemptStartCooldown <= 0f)
        {
            if (supplies.ResourceValue >= 1)
            {
                StartCreatingCure();
            }
            else
            {
                attemptStartCooldown = 0.5f;
            }
        }
        else
        {
            attemptStartCooldown -= Time.deltaTime;
        }
    }

    private void StartCreatingCure()
    {
        ResourceManager supplies = GameManager.Instance.SupplyResource;
        supplies.changeResourceLevel(-1);
        cureInProgress = true;
    }

    private void FinishCure()
    {
        cureInProgress = false;
        labProgress = 0;
        GameManager.Instance.CureResource.changeResourceLevel(1);
        attemptStartCooldown = 0.5f;
    }

    public override float GetVisualProgress()
    {
        return labProgress;
    }

}