using UnityEngine;

public class VisitorUnit : Unit
{
    public override Team Team => Team.Visitor;
    private HospitalBed targetBed;
    private float pathTimer = 0f;


    private void Update()
    {
        if (pathTimer <= 0f)
        {
            pathTimer = 1f;
            if (CurrentPath == null)
            {
                FindBed();
                if (targetBed == null) 
                {
                    Debug.Log("No bed found");
                }
            }
        }
        else
        {
            pathTimer -= Time.deltaTime;
        }

    }

    private void FindBed()
    {
        if (GameManager.Instance.AvailableBeds.Count == 0)
        {
            targetBed = null;
            return;
        }
        HospitalBed bed = GameManager.Instance.AvailableBeds[0];
        targetBed = bed;
        bed.ReservePatient(this);
        StartCoroutine(PathfindCoroutine(targetBed.Pos));
    }

}