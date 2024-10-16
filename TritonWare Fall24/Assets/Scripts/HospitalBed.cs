public class HospitalBed : Workstation
{
    public Unit Patient;
    public Unit ReservedPatient;


    public void InsertPatient(Unit unit)
    {
        Patient = unit;
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