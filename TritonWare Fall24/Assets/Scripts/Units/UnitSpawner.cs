using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : MonoBehaviour
{
    public float maxZombieSpawnInterval = 5;
    public float minZombieSpawnInterval = 1;
    public float minPatientSpawnInterval = 10;
    public float maxPatientSpawnInterval = 20;
    public float spawnRateRandomness = 0.1f;
    public float spawnPositionRandomness = 0.1f;
    private float zombieSpawnTimer = 0f;
    private float patientSpawnTimer = 0f;
    public int groupSize = 1;

    public float patientMaxInfection = 0.3f;


    public Unit StandardEnemy;
    public Unit StandardPatient;
    private List<Vector2Int> spawnablePositions;

    private float initialState;



    private void Awake()
    {
        spawnablePositions = MapManager.Instance.GetMapEdge();
        zombieSpawnTimer = minZombieSpawnInterval;
        patientSpawnTimer = 5;
        initialState = Random.value * 100000;
    }


    private void Update()
    {
        // Using Perlin Noise for random but smooth fluctuation
        float spawnRateNoise = Mathf.PerlinNoise(initialState + Time.time * spawnRateRandomness, 0);
        float patientSpawnRateNoise = Mathf.PerlinNoise(initialState + Time.time * spawnRateRandomness, 1000);
        OverlayManager.Instance.ShowZombieActivity(1 - Mathf.Clamp(
                Mathf.Lerp(-0.2f, 1.2f, spawnRateNoise), 0, 0.9f));
        if (zombieSpawnTimer <= 0f)
        {
            zombieSpawnTimer = Mathf.Clamp(
                Mathf.Lerp(0.85f * minZombieSpawnInterval, 1.15f * maxZombieSpawnInterval, spawnRateNoise),
                minZombieSpawnInterval,
                maxZombieSpawnInterval);
            zombieSpawnTimer /= GameManager.DifficultyScaling;
            SpawnGroup(spawnablePositions[Random.Range(0, spawnablePositions.Count)]);
        }
        if (patientSpawnTimer <= 0f)
        {
            patientSpawnTimer = Mathf.Lerp(minPatientSpawnInterval, maxPatientSpawnInterval, patientSpawnRateNoise) / GameManager.ReducedDifficultyScaling;
            StartCoroutine(SpawnPatientCoroutine());
        }

        if (!GameManager.Instance.isSettingUp)
        {
            zombieSpawnTimer -= Time.deltaTime;

        }
        patientSpawnTimer -= Time.deltaTime;

        // Debug.Log($"Current spawn interval: {spawnRateNoise}");

    }

    private IEnumerator SpawnPatientCoroutine()
    {
        VisitorUnit newPatient = TrySpawn(spawnablePositions, StandardPatient) as VisitorUnit;
        if (newPatient != null)
        {
            // really hacky code
            newPatient.UnPlace();
            newPatient.UnitDisplay.enabled = false;
            MapManager.Instance.GetTile(newPatient.Pos).ContainedUnit = newPatient;
            OverlayManager.Instance.CreateTargetIndicator(newPatient.Pos, TargetIndicatorType.PatientSpawn);
            yield return new WaitForSeconds(3);
            newPatient.Place(newPatient.Pos);
            newPatient.UnitDisplay.enabled = true;
            newPatient.Infection = new Infection(newPatient, Random.Range(0, patientMaxInfection * GameManager.DifficultyScaling));
        }
    }


    private void SpawnGroup(Vector2Int origin)
    {
        List<MapTile> tiles = MapManager.Instance.GetTilesInRadius(origin, 3, true);
        for (int i = 0; i < groupSize + Random.Range(-2, 3); i++)
        {
            TrySpawn(tiles, StandardEnemy);
        }

        // OverlayManager.Instance.CreateTargetIndicator(origin, TargetIndicatorType.EnemySpawn);

        /*
        OverlayManager.Instance.Targets.Enqueue(new Tuple<Vector2Int, float>(origin, Time.time));
        GameObject indicator = Instantiate(OverlayManager.Instance.enemySpawnIndicatorPrefab);
        OverlayManager.Instance.TargetIndicators.Add(origin, indicator);
        */
    }

    // tries to spawn a unit in a randomly picked position if valid
    private Unit TrySpawn(List<Vector2Int> positions, Unit unitTemplate)
    {
        int spawnAttempts = 0;
        while (spawnAttempts < 10)
        {
            int index = Random.Range(0, positions.Count);
            if (MapManager.Instance.GetTile(positions[index]).IsPassable())
            {
                return SpawnUnit(positions[index], unitTemplate);
            }
            else
            {
                spawnAttempts++;
            }
        }
        Debug.Log("Failed to find valid spawn location");
        return null;
    }

    private Unit TrySpawn(List<MapTile> tiles, Unit unitTemplate)
    {
        List<Vector2Int> positions = new();

        foreach (MapTile tile in tiles)
        {
            positions.Add(tile.Pos);
        }
        return TrySpawn(positions, unitTemplate);
    }

    private Unit SpawnUnit(Vector2Int pos, Unit unitTemplate)
    {
        Unit newUnit = Instantiate(unitTemplate);
        newUnit.Place(pos);
        return newUnit;
    }


}