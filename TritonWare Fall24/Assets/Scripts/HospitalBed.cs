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
    }

    public void RemovePatient()
    {
        Patient = null;
        GameManager.Instance.AvailableBeds.Add(this);
    }

    public void ReservePatient(Unit unit)
    {
        ReservedPatient.ClearPath();
        ReservedPatient = unit;
        if (GameManager.Instance.AvailableBeds.Contains(this))
        {
            GameManager.Instance.AvailableBeds.Remove(this);
        }
    }

}