using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance;
    public GameObject HoverOverlay;
    private void Awake()
    {
        Instance = this;
        HoverOverlay.SetActive(true);
    }



    public void HoverTile(Vector2Int pos)
    {
        HoverOverlay.transform.position = MapManager.Instance.GetWorldPos(pos);

        /*
        foreach (MapTile tile in MapManager.Instance.Tiles)
        {
            tile.SpriteRenderer.enabled = false;
        }
        foreach (MapTile tile in MapManager.Instance.GetTilesInRadius(pos, 2.5f))
        {
            tile.SpriteRenderer.enabled = true;
        }
        */
    }

    public void SelectTiles(Vector2Int pos1, Vector2Int pos2)
    {
        Vector2Int bottomLeft = new Vector2Int(
            Mathf.Min(pos1.x, pos2.x),
            Mathf.Min(pos1.y, pos2.y)
        );

        Vector2Int upperRight = new Vector2Int(
          Mathf.Max(pos1.x, pos2.x),
          Mathf.Max(pos1.y, pos2.y)
        );

        int scaleX = upperRight.x - bottomLeft.x;
        int scaleY = upperRight.y - bottomLeft.y;

        HoverOverlay.transform.localScale = new Vector2(scaleX + 1, scaleY + 1);
        HoverOverlay.transform.position = new Vector3(bottomLeft.x, bottomLeft.y, 0);
    }

    public void ResetHoverOverlay()
    {
        HoverOverlay.transform.localScale = Vector2.one;
    }


}