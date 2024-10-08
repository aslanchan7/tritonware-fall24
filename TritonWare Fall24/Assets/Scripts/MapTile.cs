using UnityEngine;

public class MapTile : MonoBehaviour
{
    public Vector2Int Pos;
    public SpriteRenderer SpriteRenderer;
    public Unit ContainedUnit;
    public Structure ContainedStructure;

    public static MapTile lastHovered = null;

    // whether movement can pass through
    public bool IsPassable()
    {
        return (ContainedUnit == null && (ContainedStructure == null || !ContainedStructure.BlocksMovement));
    }

    // whether vision can pass through
    public bool IsUnobstructed()
    {
        return ContainedStructure == null || !ContainedStructure.BlocksVision;
    }
}
