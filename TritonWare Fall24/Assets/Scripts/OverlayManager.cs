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
    }
}