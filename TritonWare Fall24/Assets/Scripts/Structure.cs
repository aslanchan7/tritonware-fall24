using UnityEngine;

public abstract class Structure : Entity
{
    public virtual void Place(Vector2Int targetPos)
    {
        MapTile targetTile = MapManager.Instance.GetTile(targetPos);
        transform.SetParent(MapManager.Instance.GetTile(targetPos).transform, false);
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                // fill up all tiles that are bounded by the size of this structure
                MapTile partialTile = MapManager.Instance.GetTile(targetPos + new Vector2Int(i, j));
                if (!partialTile.IsPassable() || partialTile.ContainedStructure != null)
                {
                    Debug.LogError("Tried to place into occupied tile");
                }
                partialTile.ContainedStructure = this;     
            }
        }
        Pos = targetPos;
    }
}