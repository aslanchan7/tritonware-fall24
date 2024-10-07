using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public List<Unit> SelectedUnits;
    public static UnitController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectUnitAtPos(Vector2Int pos)
    {
        foreach (Unit unit in SelectedUnits)
        {
            unit.SelectIndicator.enabled = false;
        }
        SelectedUnits.Clear();
        Unit foundUnit = MapManager.Instance.GetUnit(pos);
        if (foundUnit != null)
        {
            SelectedUnits.Add(foundUnit);
            foundUnit.SelectIndicator.enabled = true;
        }
    }

    public void GiveOrder(Vector2Int pos)
    {
        if (MapManager.Instance.IsPassable(pos) && SelectedUnits.Count > 0 
            && SelectedUnits[0] is AlliedUnit a)
        {
            OrderMove(pos);
        }
    }

    public void OrderMove(Vector2Int pos)
    {
        foreach (Unit unit in SelectedUnits) 
        { 
            if (((AlliedUnit)unit).IsControllable())
            {
                unit.Move(pos);
                // TODO: multiple units and collision avoidance logic
            }
        }
    }
}