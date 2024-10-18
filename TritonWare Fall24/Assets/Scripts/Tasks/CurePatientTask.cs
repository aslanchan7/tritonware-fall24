using Unity.VisualScripting;
using UnityEngine;

public class CurePatientTask : Task
{
    public override bool CancelOnInterrupt => false;
    private HospitalBed bed;
    public float cureProgress;
    private float cureSpeed = 0.01f;    // progress fraction per second


    public override void AssignTask(Unit worker)
    {
        base.AssignTask(worker);
        bed = Structure as HospitalBed;
        if (bed == null) Debug.LogError("Error matching hospital task to bed");
    }

    public override void StartTask()
    {
        base.StartTask();
        cureProgress = 0f;
    }

    public override void WorkTask()
    {
        base.WorkTask();
        if (bed.Patient != null && Worker is AlliedUnit ally)
        {
            Debug.Log(cureProgress);
            cureProgress += cureSpeed * ally.Efficiency[Tasks.Hospital] * Time.deltaTime;
            if (cureProgress >= 1f)
            {
                CurePatient();
            }
        }

    }

    private void CurePatient()
    {
        Unit patient = bed.Patient;
        cureProgress = 0f;
        patient.Infection.RemoveInfection();
        if (patient is VisitorUnit)
        {
            Debug.Log("here");
            // recruit upon cured (turn into different unit)
            bed.RemovePatient();
            patient.TurnIntoUnit(GameManager.Instance.AllUnitPrefabs().RandomElement(), 0.1f);
        }
        else
        {
            // same unit exit bed
            bed.RemovePatient();
        }

        
    }

    
}