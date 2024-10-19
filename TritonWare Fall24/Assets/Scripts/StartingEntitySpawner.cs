using JetBrains.Annotations;
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
            if (!unit.gameObject.activeSelf) continue;
            unit.transform.position = Vector3.zero;
            unit.Place(unit.Pos);
        }
        foreach (Structure structure in StructureList.GetComponentsInChildren<Structure>())
        {
            if (!structure.gameObject.activeSelf) continue;
            structure.transform.position = Vector3.zero;
            structure.Place(structure.Pos);
        }
    }

    public List<Entity> PreviewWorldPostiions()
    {
        List<Entity> changed = new List<Entity>();
        foreach (Unit unit in UnitList.GetComponentsInChildren<Unit>())
        {
            unit.transform.position = (Vector3)(Vector2)unit.Pos;
            changed.Add(unit);
        }
        foreach (Structure structure in StructureList.GetComponentsInChildren<Structure>())
        {
            structure.transform.position = (Vector3)(Vector2)structure.Pos;
            changed.Add(structure);
        }
        return changed;
    }

    public List<Entity> SetGridPosFromWorldPos()
    {
        foreach (Unit unit in UnitList.GetComponentsInChildren<Unit>())
        {
            unit.Pos = new Vector2Int(Mathf.RoundToInt(unit.transform.position.x), Mathf.RoundToInt(unit.transform.position.y));
        }
        foreach (Structure structure in StructureList.GetComponentsInChildren<Structure>())
        {
            structure.Pos = new Vector2Int(Mathf.RoundToInt(structure.transform.position.x), Mathf.RoundToInt(structure.transform.position.y));
        }
        return PreviewWorldPostiions();
    }

    public List<Entity> AllEntities()
    {
        List<Entity> changed = new List<Entity>();
        foreach (Unit unit in UnitList.GetComponentsInChildren<Unit>())
        {
            changed.Add(unit);
        }
        foreach (Structure structure in StructureList.GetComponentsInChildren<Structure>())
        {
            changed.Add(structure);
        }
        return changed;
    }
}
