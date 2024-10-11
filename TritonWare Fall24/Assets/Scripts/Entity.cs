using System.Collections.Generic;
using UnityEngine;

// anything that takes up space on a map
public abstract class Entity : MonoBehaviour
{
    // don't mind this hacky code...
    [SerializeField]
    private Vector2Int pos;
    [HideInInspector]
    public Vector2Int Pos
    {
        get { return pos; }
        set { pos = value; }
    }

    public abstract Vector2Int Size { get; }    // How many tiles does this entity extend from the bottom left corner

    public abstract bool BlocksMovement { get; }
    public abstract bool BlocksVision { get; }
    public abstract Team Team { get; }

    public List<Vector2Int> GetOccupiedPositions()
    {
        List<Vector2Int> result = new();
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                result.Add(Pos + new Vector2Int(i, j));
            }
        }
        return result;
    }
}