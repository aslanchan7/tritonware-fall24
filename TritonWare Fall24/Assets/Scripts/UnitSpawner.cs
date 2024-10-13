using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public float maxSpawnInterval = 5;
    public float minSpawnInterval = 1;
    public float spawnRateRandomness = 0.1f;
    public float spawnPositionRandomness = 0.1f;
    private float spawnTimer = 0f;
    public int groupSize = 1;

    public Unit StandardEnemy;
    private List<Vector2Int> spawnablePositions;

    private void Awake()
    {
        spawnablePositions = MapManager.Instance.GetMapEdge();
    }


    private void Update()
    {
        // Using Perlin Noise for random but smooth fluctuation
        float spawnRateNoise = Mathf.PerlinNoise(Time.time * spawnRateRandomness, 0);
        float currentSpawnInterval = Mathf.Lerp(minSpawnInterval, maxSpawnInterval, spawnRateNoise);  // Interpolate between minRate and maxRate

        float spawnPositionNoise = Mathf.PerlinNoise(Time.time * spawnPositionRandomness, 100);
        int spawnPosition = (int)Mathf.Lerp(0, spawnablePositions.Count, spawnPositionNoise);

        if (spawnTimer <= 0f)
        {
            spawnTimer = currentSpawnInterval;
            SpawnGroup(spawnablePositions[spawnPosition]);

            // todo if spawn position is occupied try again
        }
        spawnTimer -= Time.deltaTime;

        Debug.Log($"Current spawn interval: {currentSpawnInterval}, Current spawn position: {spawnPosition}");

    }


    private void SpawnGroup(Vector2Int origin)
    {
        List<MapTile> positions = MapManager.Instance.GetTilesInRadius(origin, 3);
        for (int i = 0; i < groupSize; i++)
        {
            int index = Random.Range(0, positions.Count);
            if (positions[index].IsPassable()) 
            {
                SpawnUnit(positions[index].Pos, StandardEnemy);
            }
            else
            {
                Debug.LogWarning("impassable");
            }
        }
    }

    private void SpawnUnit(Vector2Int pos, Unit unitTemplate)
    {
        Unit newUnit = Instantiate(unitTemplate);
        newUnit.Place(pos);
    }




}