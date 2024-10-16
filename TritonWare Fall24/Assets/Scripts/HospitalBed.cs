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
    }

    public void RemovePatient()
    {
        Patient = null;
        GameManager.Instance.AvailableBeds.Add(this);
    }

    public void ReservePatient(Unit unit)
    {
        if (ReservedPatient != null)
        {
            ReservedPatient.ClearPath();
        }
        
        ReservedPatient = unit;
        if (GameManager.Instance.AvailableBeds.Contains(this))
        {
            GameManager.Instance.AvailableBeds.Remove(this);
        }
    }

}