using UnityEngine;
using UnityEditor;  // Import Editor tools

[CustomEditor(typeof(MapManager))]
public class MapEditor : Editor
{
    bool editorControlsEnabled = false;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();  // Draw the default inspector UI

        MapManager myScript = (MapManager)target;
        
        editorControlsEnabled = GUILayout.Toggle(editorControlsEnabled, "Enable Editor Controls (Use Caution)");

        if (editorControlsEnabled)
        {
            // Add a button to the editor inspector
            if (GUILayout.Button("Draw Rectangle of Tiles"))
            {
                myScript.CreateTileMap();  // Call the rectangle drawing function
            }
            if (GUILayout.Button("Create Game Grid"))
            {
                myScript.InitGameGrid();
            }
            if (GUILayout.Button("Clear Map"))
            {
                myScript.ClearMap();
            }
        }


    }
}