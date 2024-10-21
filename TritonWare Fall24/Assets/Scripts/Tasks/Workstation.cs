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
    private Task taskInProgress;

    public Task TaskInProgress { get => taskInProgress; set { taskInProgress = value; } }

    public bool IsEnabled = true;
    public Vector2Int GetWorkPos()
    {
        return Pos + RelativeWorkTilePos;
    }

    public override void Place(Vector2Int targetPos)
    {
        base.Place(targetPos);
        WorkTileIcon.transform.position = MapManager.GetTileCenter(GetWorkPos());
        ToggleEnabled(IsEnabled);
    }

    public Task PrepareWorkstationTask(Unit worker)   // creates a new task or resumes a previously started task for the assigned unit but do not start it until unit reaches WorkTile
    {
        if (TaskInProgress == null)
        {
            Task task = WorkstationTaskTemplate.CreateTask(this);
            TaskInProgress = task;
            task.ValidWorkingPositions.Add(GetWorkPos());
            task.transform.SetParent(transform, false);
            task.AssignTask(worker);
            return task;
            
        }
        else
        {
            TaskInProgress.AssignTask(worker);
            return TaskInProgress;
        }

    }

    public void ToggleEnabled(bool enabled, bool external = true)
    {
        IsEnabled = enabled;
        if (enabled)
        {
            MapManager.Instance.GetTile(GetWorkPos()).ReservingWorkstation = this;
            WorkTileIcon.enabled = true;
        }
        else
        {
            // external should be false if ToggleEnabled is triggered by the task itself to avoid recursion
            if (external && TaskInProgress != null) TaskInProgress.RemoveTask();
            MapManager.Instance.GetTile(GetWorkPos()).ReservingWorkstation = null;
            WorkTileIcon.enabled = false;
        }
    }


}