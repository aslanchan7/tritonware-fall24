using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePickup : Entity
{
    public override bool BlocksMovement => false;
    public override bool BlocksVision => false;
    public override Vector2Int Size => new Vector2Int(1, 1);
    public override Team Team => Team.Neutral;

    public ResourceManager ResourceType;
    public int Value;

    public SpriteRenderer spriteRenderer;
    public List<Sprite> spriteVariants = new();

    public void Awake()
    {
        spriteRenderer.sprite = spriteVariants.RandomElement();
    }

    public void Place(Vector2Int pos)
    {
        Pos = pos;
        MapTile targetTile = MapManager.Instance.GetTile(pos);
        transform.SetParent(MapManager.Instance.GetTile(pos).transform, false);
        if (targetTile.ContainedUnit != null)
        {
            Debug.LogError("Tried to spawn into tile occupied by unit");
        }
        targetTile.ContainedResource = this;
        GameManager.AllResourcesPickups.Add(this);
        ResourceType = GameManager.Instance.SupplyResource;
    }

    public void Pickup()
    {
        ResourceType.changeResourceLevel(Value);
        GameManager.AllResourcesPickups.Remove(this);
        Destroy(gameObject);
    }

}
