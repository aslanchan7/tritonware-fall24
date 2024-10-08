using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

// Looks for units in its children, then moves them to their correct positions at game startup
public class StartingUnitSpawner : MonoBehaviour
{
    private void Awake()
    {
        foreach (Unit unit in GetComponentsInChildren<Unit>())
        {
            MapTile targetTile = MapManager.Instance.GetTile(unit.Pos);
            unit.transform.SetParent(MapManager.Instance.GetTile(unit.Pos).transform, false);
            if (targetTile.ContainedUnit != null)
            {
                Debug.LogError("Tried to spawn into tile occupied by unit");
            }
            targetTile.ContainedUnit = unit;
        }
    }
}
