using UnityEngine;

public class MapTile : MonoBehaviour
{
    public Vector2Int Pos;
    public SpriteRenderer SpriteRenderer;
    public Unit ContainedUnit;

    public static MapTile lastHovered = null;

}
