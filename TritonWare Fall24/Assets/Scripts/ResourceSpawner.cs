using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public float maxSpawnInterval = 5;
    public float minSpawnInterval = 1;
    public float spawnRateRandomness = 0.1f;
    public float spawnPositionRandomness = 0.1f;
    private float spawnTimer = 0f;
    public int groupSize = 1;

    public ResourcePickup ResourceUnit;
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

        if (spawnTimer <= 0f)
        {
            spawnTimer = currentSpawnInterval;
            SpawnGroup(spawnablePositions[Random.Range(0, spawnablePositions.Count)]);

            // todo if spawn position is occupied try again
        }
        if (GameManager.AllResourcesPickups.Count <= 6)
        {
            spawnTimer -= Time.deltaTime;   // stop spawning when there is already a lot of resources
        }
    }


    private void SpawnGroup(Vector2Int origin)
    {
        List<MapTile> positions = MapManager.Instance.GetTilesInRadius(origin, 3);
        for (int i = 0; i < groupSize; i++)
        {
            int index = Random.Range(0, positions.Count);
            if (positions[index].IsPassable() && positions[index].ContainedResource == null) 
            {
                SpawnResource(positions[index].Pos, ResourceUnit);
            }
            else
            {
                Debug.LogWarning("invalid spawn location");
            }
        }
    }

    private void SpawnResource(Vector2Int pos, ResourcePickup unitTemplate)
    {
        ResourcePickup newUnit = Instantiate(unitTemplate);
        newUnit.Place(pos);
    }
}
