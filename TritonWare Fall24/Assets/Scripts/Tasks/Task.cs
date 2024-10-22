using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Task : MonoBehaviour
{
    [HideInInspector] public bool IsTemplate = true;
    public Unit Worker;
    [HideInInspector] public bool IsStarted = false;
    [HideInInspector] public bool IsWorking = false;
    public abstract bool CancelOnInterrupt { get; }
    public List<Vector2Int> ValidWorkingPositions = new List<Vector2Int>();
    public Structure Structure;
    public ProgressBar ProgressBar = null;
    public string Description = "Begin Task";


    private void Awake()
    {

    }



    private void Update()
    {
        if (IsWorking) WorkTask();
        if (ProgressBar != null && GetVisualProgress() >= 0f)
        {
            ProgressBar.SetProgress(GetVisualProgress());
        }
    }

    public Task CreateTask(Structure s = null)
    {
        Task clone = Instantiate(this);
        clone.IsTemplate = false;
        clone.Structure = s;
        
        if (clone.ProgressBar != null && clone.Structure != null)
        {
            clone.ProgressBar.transform.localPosition = clone.Structure.GetRelativeWorldCenter() + new Vector3(0, 0.9f, 0);
        }
        return clone;
    }

    public virtual void AssignTask(Unit worker) // adds this task to the work queue of the targeted unit but do not start it yet
    {
        if (IsTemplate) Debug.LogWarning("Using Template Task instead of Instantiated Task");
        Worker = worker;
        worker.EnqueueTask(this);
    }

    public virtual void StartTask()             // starts progress on the task
    {
        if (IsTemplate) Debug.LogWarning("Using Template Task instead of Instantiated Task");
        IsStarted = true;
        IsWorking = true;
    }

    public virtual void WorkTask()      // called every frame
    {

    }

    public virtual void PauseTask()     // pauses the task in progress and unassigns the worker but do not destroy this task unless not started or CancelOnInterrupt is true
    {
        IsWorking = false;
        if (Worker != null) Worker.TaskQueue.Remove(this);
        Worker = null;
        if (!IsStarted || CancelOnInterrupt) RemoveTask();
    }

    public virtual void FinishTask()       // completes a task, triggering its completion effects and destroying it afterwards
    {
        // individual task code
        RemoveTask();
    }

    public virtual void RemoveTask()       // destroys an ongoing or finished task
    {
        Debug.Log("remove task " + name);
        if (Worker != null) Worker.TaskQueue.Remove(this);
        if (Structure is Workstation w && w.TaskInProgress == this) w.TaskInProgress = null;
        Destroy(gameObject);
    }

    public virtual float GetVisualProgress() 
    {
        return -1f;
    }

    public virtual void ResetTask() { }

}