using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public void RenderTrail(Vector2 pos1, Vector2 pos2, float lifetime)
    {
        lineRenderer.SetPosition(0, pos1);
        lineRenderer.SetPosition(1, pos2);
        Destroy(gameObject, lifetime);
    }
}
