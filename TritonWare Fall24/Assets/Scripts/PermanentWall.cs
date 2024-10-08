using UnityEngine;

// Indestructible impassable wall that takes up one tile
public class PermanentWall : Structure
{
    public override Vector2Int Size => new Vector2Int(1, 1);
    public override bool BlocksMovement => true;
    public override bool BlocksVision => true;
    public override Team Team => Team.Neutral;
}