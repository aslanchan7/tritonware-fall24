using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

// Looks for entities in its children, then moves them to their correct positions at game startup
public class StartingEntitySpawner : MonoBehaviour
{
    public GameObject UnitList;
    public GameObject StructureList;
    public static StartingEntitySpawner Instance;
    private void Awake()
    {
        Instance = this;
        SpawnEntities();
    }

    public void SpawnEntities()
    {
        foreach (Unit unit in UnitList.GetComponentsInChildren<Unit>())
        {
            unit.transform.position = Vector3.zero;
            MapTile targetTile = MapManager.Instance.GetTile(unit.Pos);
            unit.transform.SetParent(MapManager.Instance.GetTile(unit.Pos).transform, false);
            if (targetTile.ContainedUnit != null)
            {
                Debug.LogError("Tried to spawn into tile occupied by unit");
            }
            targetTile.ContainedUnit = unit;
        }
        foreach (Structure structure in StructureList.GetComponentsInChildren<Structure>())
        {
            structure.transform.position = Vector3.zero;
            structure.Place(structure.Pos);
        }
    }

    public void PreviewWorldPostiions()
    {
        foreach (Unit unit in UnitList.GetComponentsInChildren<Unit>())
        {
            unit.transform.position = (Vector3)(Vector2)unit.Pos;
        }
        foreach (Structure structure in StructureList.GetComponentsInChildren<Structure>())
        {
            structure.transform.position = (Vector3)(Vector2)structure.Pos;
        }
    }
}
