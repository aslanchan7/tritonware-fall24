#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;  // Import Editor tools

[CustomEditor(typeof(StartingEntitySpawner))]
public class EntitySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();  // Draw the default inspector UI

        StartingEntitySpawner myScript = (StartingEntitySpawner)target;

        // Add a button to the editor inspector
        if (GUILayout.Button("Preview World Positions in Editor View"))
        {
            // Record the object for undo support and mark as dirty
            foreach (var g in myScript.AllEntities())
            {
                Undo.RecordObject(g, "Preview World Positions");
            }

            List<Entity> changed = myScript.PreviewWorldPostiions();
            foreach (var gameObject in changed)
            {
                EditorUtility.SetDirty(gameObject);
            }
        }

        if (GUILayout.Button("Interpret Grid Positions from World Positions"))
        {
            // Record the object for undo support and mark as dirty
            foreach (var g in myScript.AllEntities())
            {
                Undo.RecordObject(g, "Preview World Positions");
            }

            List<Entity> changed = myScript.SetGridPosFromWorldPos();
            foreach (var gameObject in changed)
            {
                EditorUtility.SetDirty(gameObject);
            }
        }
    }
}
#endif