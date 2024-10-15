using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance;
    public GameObject HoverOverlay;

    // Enemy Spawn Indicators
    public GameObject enemySpawnIndicatorPrefab;
    public Queue<Tuple<Vector2Int, float>> Targets = new();
    public Dictionary<Vector2Int, GameObject> TargetIndicators = new();
    private Camera cam;
    private SpriteRenderer indicatorSpriteRenderer;
    private float indicatorSpriteWidth;
    private float indicatorSpriteHeight;

    private void Awake()
    {
        Instance = this;
        HoverOverlay.SetActive(true);

        cam = Camera.main;

        indicatorSpriteRenderer = enemySpawnIndicatorPrefab.GetComponent<SpriteRenderer>();
        indicatorSpriteWidth = indicatorSpriteRenderer.bounds.size.x / 2f;
        indicatorSpriteHeight = indicatorSpriteRenderer.bounds.size.y / 2f;
    }

    private void Update()
    {
        // Every frame update the indicator's position
        foreach (KeyValuePair<Vector2Int, GameObject> entry in TargetIndicators)
        {
            Vector2Int target = entry.Key;
            GameObject indicator = entry.Value;

            UpdateTargetIndicator(target, indicator);
        }

        if (Targets.Count > 0)
        {
            // After 3 seconds, destroy the indicator game object
            if (Time.time - Targets.Peek().Item2 > 3f)
            {
                Vector2Int key = Targets.Dequeue().Item1;
                Destroy(TargetIndicators.GetValueOrDefault(key));
                TargetIndicators.Remove(key);
            }
        }
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

    public void UpdateTargetIndicator(Vector2Int target, GameObject indicator)
    {
        Vector3 targetPos = MapManager.Instance.GetWorldPos(target);

        // Takes the world pos and converts it into a position relative to camera
        // Bottom left of the camera is (0,0) and top right is (1,1)
        Vector3 screenPos = cam.WorldToViewportPoint(targetPos);
        bool isOffScreen = screenPos.x <= 0f || screenPos.x >= 1f || screenPos.y <= 0f || screenPos.y >= 1f;

        if (isOffScreen)
        {
            indicator.SetActive(true);
            Vector3 spriteSizeInViewPort = cam.WorldToViewportPoint(new Vector3(indicatorSpriteWidth, indicatorSpriteHeight, 0)) - cam.WorldToViewportPoint(Vector3.zero);

            screenPos.x = Mathf.Clamp(screenPos.x, spriteSizeInViewPort.x, 1 - spriteSizeInViewPort.x);
            screenPos.y = Mathf.Clamp(screenPos.y, spriteSizeInViewPort.y, 1 - spriteSizeInViewPort.y);

            Vector3 worldPos = cam.ViewportToWorldPoint(screenPos);
            worldPos.z = 0;
            indicator.transform.position = worldPos;
        }
        else
        {
            indicator.SetActive(false);
        }
    }
}