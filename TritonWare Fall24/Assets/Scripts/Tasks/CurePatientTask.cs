using Unity.VisualScripting;
using UnityEngine;

public class CurePatientTask : Task
{
    public override bool CancelOnInterrupt => false;
    private HospitalBed bed;
    public float cureProgress;
    public float cureSpeed = 0.03f;    // progress fraction per second
    private bool consumedCure = false;

    public override void AssignTask(Unit worker)
    {
        base.AssignTask(worker);
        bed = Structure as HospitalBed;
        if (bed == null) Debug.LogError("Error matching hospital task to bed");
    }

    public override void StartTask()
    {
        base.StartTask();
    }

    public override void WorkTask()
    {
        base.WorkTask();
        if (bed.Patient != null && Worker is AlliedUnit ally)
        {
            if (!consumedCure && GameManager.Instance.CureResource.ResourceValue >= 1)
            {
                consumedCure = true;
                GameManager.Instance.CureResource.changeResourceLevel(-1);
            }
            else if (consumedCure) 
            {
                cureProgress += cureSpeed * ally.Efficiency[Tasks.Hospital] * Time.deltaTime;
                if (cureProgress >= 1f)
                {
                    CurePatient();
                }
            }
        }

    }

    private void CurePatient()
    {
        Unit patient = bed.Patient;
        cureProgress = 0f;
        patient.Infection.RemoveInfection();
        Debug.Log("cured " + bed.Patient.name);
        if (patient is VisitorUnit)
        {
            // recruit upon cured (turn into different unit)
            bed.RemovePatient();
            patient.TurnIntoUnit(GameManager.Instance.GetWeightedUnitDraw(), 0.1f);
            GameManager.Instance.PatientsCured++;
        }
        else
        {
            // same unit exit bed
            bed.RemovePatient();
        }
    }

    public override void RemoveTask()
    {
        cureProgress = 0f;
        base.RemoveTask();
    }

    public override void ResetTask()
    {
        cureProgress = 0f;
        consumedCure = false;
        base.ResetTask();


    }

    public override float GetVisualProgress()
    {
        return cureProgress;
    }

}