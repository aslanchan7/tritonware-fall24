using TMPro;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public Vector2Int Pos;
    public SpriteRenderer SpriteRenderer;
    public Unit ContainedUnit;
    public Resource ContainedResource;
    public Structure ContainedStructure;
    public Workstation ReservingWorkstation;

    public static MapTile lastHovered = null;

    public TMP_Text DebugText;


    // whether movement can pass through
    public bool IsPassable()
    {
        return (ContainedUnit == null &&                                                    // there is no unit in the way
                (ContainedStructure == null || !ContainedStructure.BlocksMovement));        // there is no structure in the way that blocks movement
                //ReservingWorkstation == null);                                              // there is no workstation that reserves this tile as a WorkTile
    }

    // whether vision can pass through
    public bool IsUnobstructed()
    {
        return ContainedStructure == null || !ContainedStructure.BlocksVision;
    }

    public void SetDebugText(string text)
    {
        DebugText.text = text;
    }
}
