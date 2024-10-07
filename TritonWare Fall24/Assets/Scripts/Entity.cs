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

    public abstract bool BlocksMovement { get; }
    public abstract bool BlocksVision { get; }
    public abstract Team Team { get; }
}