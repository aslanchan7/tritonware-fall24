using System;
using System.Collections.Generic;
using System.Linq;
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
            SpawnUnit(spawnablePositions[Random.Range(0, spawnablePositions.Count)], StandardPatient);
        }

        zombieSpawnTimer -= Time.deltaTime;

        // Debug.Log($"Current spawn interval: {currentSpawnInterval}");

    }


    private void SpawnGroup(Vector2Int origin)
    {
        List<MapTile> positions = MapManager.Instance.GetTilesInRadius(origin, 3);
        for (int i = 0; i < groupSize + Random.Range(-2, 3); i++)
        {
            int index = Random.Range(0, positions.Count);
            if (positions[index].IsPassable())
            {
                SpawnUnit(positions[index].Pos, StandardEnemy);
            }
            else
            {
                Debug.LogWarning("invalid spawn location");
            }
        }

        OverlayManager.Instance.Targets.Enqueue(new Tuple<Vector2Int, float>(origin, Time.time));
        GameObject indicator = Instantiate(OverlayManager.Instance.enemySpawnIndicatorPrefab);
        OverlayManager.Instance.TargetIndicators.Add(origin, indicator);
    }

    private void SpawnUnit(Vector2Int pos, Unit unitTemplate)
    {
        Unit newUnit = Instantiate(unitTemplate);
        newUnit.Place(pos);
    }




}