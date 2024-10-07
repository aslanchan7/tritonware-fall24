using UnityEngine;
using UnityEditor;  // Import Editor tools

[CustomEditor(typeof(MapManager))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();  // Draw the default inspector UI

        MapManager myScript = (MapManager)target;
        // Add a button to the editor inspector
        if (GUILayout.Button("Draw Rectangle of Tiles"))
        {
            myScript.CreateTileMap();  // Call the rectangle drawing function
        }
        if (GUILayout.Button("Clear Tiles"))
        {
            myScript.ClearTiles();
        }
    }
}