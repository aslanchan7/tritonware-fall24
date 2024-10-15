using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public TrailRenderer projectilePrefab;
    public Dictionary<TrailRenderer, Vector2> trails = new();

    public float speed;
    private List<TrailRenderer> toDestroy = new List<TrailRenderer>();


    public void RenderTrail(Vector2 pos1, Vector2 pos2, float lifetime)
    {
        lineRenderer.SetPosition(0, pos1);
        lineRenderer.SetPosition(1, pos2);
        Destroy(gameObject, lifetime);
    }

    public void RenderProjectile(Vector2 pos1, Vector2 pos2)
    {
        TrailRenderer newProj = Instantiate(projectilePrefab);
        newProj.transform.position = pos1;
        trails.Add(newProj, pos2);
    }

    private void Update()
    {
        foreach (var trail in trails)
        {
            trail.Key.transform.position = Vector2.MoveTowards(trail.Key.transform.position, trail.Value, speed * Time.deltaTime);
            if (Vector2.Distance(trail.Key.transform.position, trail.Value) <= 0.2f)
            {
                toDestroy.Add(trail.Key);
            }
        }
        while (toDestroy.Count > 0)
        { 
            trails.Remove(toDestroy[^1]);
            Destroy(toDestroy[^1].gameObject,0.1f);
            toDestroy.RemoveAt(toDestroy.Count - 1);
        }
    }
}
