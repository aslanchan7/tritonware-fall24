using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorativeStructure : Structure
{
    public Vector2Int structureSize;
    public override Vector2Int Size => structureSize;

    public override bool BlocksMovement => true;

    public override bool BlocksVision => false;

    public override Team Team => Team.Neutral;
}
