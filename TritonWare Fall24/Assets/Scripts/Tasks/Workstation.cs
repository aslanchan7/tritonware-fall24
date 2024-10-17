using UnityEngine;

public class Workstation : Structure
{
    [SerializeField] private Vector2Int workstationSize = new Vector2Int(1, 1);   
    public override Vector2Int Size => workstationSize;
    public override bool BlocksMovement => true;
    public override bool BlocksVision => false;
    public override Team Team => Team.Allied;

    public Vector2Int RelativeWorkTilePos = new Vector2Int(0, -1); // position where a unit stands to work relative to bottom left tile
    public SpriteRenderer WorkTileIcon;
    public Task WorkstationTaskTemplate;
    public Task TaskInProgress;

    public Vector2Int GetWorkPos()
    {
        return Pos + RelativeWorkTilePos;
    }

    public override void Place(Vector2Int targetPos)
    {
        base.Place(targetPos);
        MapManager.Instance.GetTile(GetWorkPos()).ReservingWorkstation = this;
        WorkTileIcon.transform.position = MapManager.GetTileCenter(GetWorkPos());
        WorkTileIcon.enabled = true;
    }

    public void PrepareTask(Unit worker)   // creates a new task or resumes a previously started task for the assigned unit but do not start it until unit reaches WorkTile
    {
        if (TaskInProgress == null)
        {
            Task task = WorkstationTaskTemplate.CreateTask(this);
            TaskInProgress = task;
            task.ValidWorkingPositions.Add(GetWorkPos());
            task.transform.SetParent(transform, false);
            task.AssignTask(worker);
            
        }
        else
        {
            TaskInProgress.AssignTask(worker);
        }

    }



}