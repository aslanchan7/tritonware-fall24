using UnityEngine;
using UnityEditor;  // Import Editor tools

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
            myScript.PreviewWorldPostiions();
        }


    }
}