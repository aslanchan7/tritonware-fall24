using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public enum TargetIndicatorType
{
    LowHealth, PatientSpawn, ResourceSpawn, ZombieTurn
}

[Serializable]
public class IndicatorSprite
{
    public TargetIndicatorType IndicatorType;
    public Indicator IndicatorPrefab;



    public IndicatorSprite(TargetIndicatorType indicator, Indicator indicatorPrefab)
    {
        IndicatorType = indicator;
        IndicatorPrefab = indicatorPrefab;
    }
}

public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance;
    public GameObject HoverOverlay;

    // Enemy Spawn Indicators
    public GameObject enemySpawnIndicatorPrefab;
    public Queue<Tuple<Vector2Int, float>> Targets = new();
    public Dictionary<Vector2Int, Indicator> TargetIndicators = new();
    private Camera cam;
    private SpriteRenderer indicatorSpriteRenderer;
    private float indicatorSpriteWidth;
    private float indicatorSpriteHeight;

    public List<IndicatorSprite> IndicatorPrefabs;

    public Transform IndicatorContainer;
    public List<GameObject> UIObjects;
    public GameObject GameOverScreen;
    public TMP_Text scoreText, timeText, curesText, difficutyText;

    public Transform ZombieActivityBar;

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
        foreach (KeyValuePair<Vector2Int, Indicator> entry in TargetIndicators)
        {
            Vector2Int target = entry.Key;
            Indicator indicator = entry.Value;

            UpdateTargetIndicator(target, indicator);
        }

        if (Targets.Count > 0)
        {
            // After 3 seconds, destroy the indicator game object
            if (Time.time - Targets.Peek().Item2 > 3f)
            {
                Vector2Int key = Targets.Dequeue().Item1;
                Destroy(TargetIndicators.GetValueOrDefault(key).gameObject);
                TargetIndicators.Remove(key);
            }
        }
    }

    public void HoverTile(Vector2Int pos)
    {
        HoverOverlay.transform.position = MapManager.Instance.GetWorldPos(pos);
        MapTile tile = MapManager.Instance.GetTile(pos);
        if (UnitController.Instance.SelectedUnits.Count > 0)
        { 
            if (tile.ReservingWorkstation != null)
            {
                Tooltip.Instance.SetTooltipText(tile.ReservingWorkstation.WorkstationTaskTemplate.Description);
                return;
            }
            else if (tile.ContainedStructure != null)
            {
                if (tile.ContainedStructure.StructureTaskTemplate != null)
                {
                    Tooltip.Instance.SetTooltipText(tile.ContainedStructure.StructureTaskTemplate.Description);
                    return;
                }
            }
            else if (tile.ContainedResource != null)
            {
                Tooltip.Instance.SetTooltipText("Pick up Supplies");
                return;
            }
        }
        if (tile.ContainedUnit != null)
        {
            TooltipType type;
            switch (tile.ContainedUnit.Team)
            {
                case Team.Allied: type = TooltipType.Ally; break;
                case Team.Visitor: type = TooltipType.Patient; break;
                case Team.Enemy: type = TooltipType.Enemy; break;
                default: type = TooltipType.Neutral; break;
            }
            Tooltip.Instance.SetTooltipText(tile.ContainedUnit.Description, type);
            return;
        }
        else if (tile.ContainedStructure != null)
        {
            Tooltip.Instance.SetTooltipText(tile.ContainedStructure.Description, TooltipType.Neutral);
            return;
        }
        else if (tile.ContainedResource != null)
        {
            Tooltip.Instance.SetTooltipText("Supplies", TooltipType.Patient);
            return;
        }
        else Tooltip.Instance.SetEnabled(false);
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

    public void CreateTargetIndicator(Vector2Int pos, TargetIndicatorType indicatorType)
    {
        Targets.Enqueue(new Tuple<Vector2Int, float>(pos, Time.time));
        IndicatorSprite targetIndicator = new(TargetIndicatorType.LowHealth, null);
        foreach (IndicatorSprite kvp in IndicatorPrefabs)
        {
            if (kvp.IndicatorType == indicatorType)
            {
                targetIndicator = kvp;
                break;
            }
        }
        Indicator indicatorObject = Instantiate(targetIndicator.IndicatorPrefab);
        indicatorObject.transform.SetParent(IndicatorContainer);
        TargetIndicators.Add(pos, indicatorObject);
    }

    public void UpdateTargetIndicator(Vector2Int target, Indicator indicator)
    {
        Vector3 targetPos = MapManager.Instance.GetWorldPos(target).GetTileCenter() + new Vector2(0,2);

        targetPos.x = Mathf.Clamp(targetPos.x, MapManager.Instance.MapBoundary, MapManager.Instance.MapSize.x - MapManager.Instance.MapBoundary);
        targetPos.y = Mathf.Clamp(targetPos.y, MapManager.Instance.MapBoundary, MapManager.Instance.MapSize.y - MapManager.Instance.MapBoundary);

        // Takes the world pos and converts it into a position relative to camera
        // Bottom left of the camera is (0,0) and top right is (1,1)
        Vector3 screenPos = cam.WorldToViewportPoint(targetPos);
        bool isOffScreen = screenPos.x <= 0f || screenPos.x >= 1f || screenPos.y <= 0f || screenPos.y >= 1f;

        if (isOffScreen || indicator.ShowIfOnScreen)
        {
            indicator.gameObject.SetActive(true);
            Vector3 spriteSizeInViewPort = cam.WorldToViewportPoint(new Vector3(indicatorSpriteWidth, indicatorSpriteHeight, 0)) - cam.WorldToViewportPoint(Vector3.zero);

            screenPos.x = Mathf.Clamp(screenPos.x, spriteSizeInViewPort.x, 1 - spriteSizeInViewPort.x);
            screenPos.y = Mathf.Clamp(screenPos.y, spriteSizeInViewPort.y, 1 - spriteSizeInViewPort.y);



            Vector3 worldPos = cam.ViewportToWorldPoint(screenPos);
            worldPos.z = 0;
            indicator.transform.position = worldPos;

            if (!isOffScreen)
            {
                indicator.SpriteRenderer.color = new(1, 1, 1, 0.5f);
            }
            else
            {
                indicator.SpriteRenderer.color = Color.white;
            }

        }
        else
        {
            indicator.gameObject.SetActive(false);
        }
    }

    public void ShowGameOverScreen(int score, float time, int cures)
    {
        foreach (var obj in UIObjects)
        {
            obj.SetActive(false);
        }
        CameraController.Instance.mouseControlsEnabled = false;

        GameOverScreen.SetActive(true);
        scoreText.SetText(score.ToString());
        timeText.SetText(FormatTime(time));
        curesText.SetText(cures.ToString());
        difficutyText.SetText(SceneController.Instance.SelectedDifficulty.Name);
    }

    public string FormatTime(float timeInSeconds)
    {
        // Convert timeInSeconds into minutes and seconds
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);

        // Format time as "minutes:seconds"
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1.0f;
        SceneController.Instance.ReturnToMenu();
    }

    public void ShowZombieActivity(float activity)
    {
        ZombieActivityBar.localScale = new Vector3(activity, 1, 1);
    }
}