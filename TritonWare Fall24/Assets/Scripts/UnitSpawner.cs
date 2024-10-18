using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;
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

    private void Awake()
    {
        spawnablePositions = MapManager.Instance.GetMapEdge();
        patientSpawnTimer = maxPatientSpawnInterval;
    }


    private void Update()
    {
        // Using Perlin Noise for random but smooth fluctuation
        float spawnRateNoise = Mathf.PerlinNoise(Time.time * spawnRateRandomness, 0);

        if (zombieSpawnTimer <= 0f)
        {
            zombieSpawnTimer = Mathf.Lerp(minZombieSpawnInterval, maxZombieSpawnInterval, spawnRateNoise); ;
            SpawnGroup(spawnablePositions[Random.Range(0, spawnablePositions.Count)]);
            // todo if spawn position is occupied try again
        }
        if (patientSpawnTimer <= 0f)
        {
            patientSpawnTimer = Mathf.Lerp(minPatientSpawnInterval, maxPatientSpawnInterval, spawnRateNoise);
            VisitorUnit newPatient = TrySpawn(spawnablePositions, StandardPatient) as VisitorUnit;
            if (newPatient != null) newPatient.Infection = new Infection(newPatient, Random.Range(0, patientMaxInfection));
        }

        zombieSpawnTimer -= Time.deltaTime;
        patientSpawnTimer -= Time.deltaTime;

        // Debug.Log($"Current spawn interval: {currentSpawnInterval}");

    }


    private void SpawnGroup(Vector2Int origin)
    {
        List<MapTile> tiles = MapManager.Instance.GetTilesInRadius(origin, 3);
        for (int i = 0; i < groupSize + Random.Range(-2, 3); i++)
        {
            TrySpawn(tiles, StandardEnemy);
        }

        OverlayManager.Instance.CreateTargetIndicator(origin, TargetIndicator.EnemySpawn);

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
        if (unitTemplate is VisitorUnit)
        {
            OverlayManager.Instance.CreateTargetIndicator(pos, TargetIndicator.PatientSpawn);
        }
        return newUnit;
    }




}