using Unity.Collections;
using UnityEngine;

public class HospitalBed : Workstation
{
    public Unit Patient;
    public Unit ReservedPatient;


    public void InsertPatient(Unit unit)
    {
        Patient = unit;
        ReservedPatient = null;
        if (GameManager.Instance.AvailableBeds.Contains(this))
        {
            GameManager.Instance.AvailableBeds.Remove(this);
        }
        Debug.Log($"Inserted unit {unit.name}");
        unit.UnPlace();
        unit.transform.SetParent(transform, false);
        unit.transform.position = MapManager.Instance.GetWorldPos(Pos);
        unit.transform.localPosition = Vector2.zero;
        unit.InsideStructure = this;
    }

    public void RemovePatient()
    {
        Debug.Log("successfuly removed");
        var positions = GetSurroundingTiles();
        if (positions.Count == 0)
        {
            Debug.LogError("No position to remove patient");
            return;
        }
        Patient.InsideStructure = null;
        Patient.Place(positions[0]);
        if (Patient is VisitorUnit v) v.TryFindBed();
        Patient = null;
        GameManager.Instance.AvailableBeds.Add(this);
        if (TaskInProgress != null)
        {
            TaskInProgress.ResetTask();
            Debug.Log("here");
        }

    }

    public void ReservePatient(Unit unit)
    {
        if (ReservedPatient != null)
        {
            ReservedPatient.ClearPath();
            ReservedPatient.ClearTasks();
        }

        ReservedPatient = unit;
        if (GameManager.Instance.AvailableBeds.Contains(this))
        {
            GameManager.Instance.AvailableBeds.Remove(this);
        }
    }

}