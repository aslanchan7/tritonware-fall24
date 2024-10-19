using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        Handles.color = Color.red;
        FieldOfView fow = (FieldOfView)target;
        Vector3 origin = new(fow.transform.position.x + 0.5f, fow.transform.position.y + 0.5f, 0);
        Handles.DrawWireArc(origin, fow.transform.forward, -fow.transform.right, 360, fow.viewRadius);

        Handles.color = Color.black;
        foreach (Transform visibleTarget in fow.visibleTargets)
        {
            if (visibleTarget == null) continue;

            Vector3 targetPos = new(visibleTarget.position.x + 0.5f, visibleTarget.position.y + 0.5f, 0);
            Handles.DrawLine(origin, targetPos);
        }
    }
}
