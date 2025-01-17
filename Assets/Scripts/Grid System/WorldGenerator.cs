﻿using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // Active instance
    public static WorldGenerator active;

    // List of resource tiles
    public Tilemap resourceGrid;
    public int borderSize = 750;
    public float perlinScale = 500;
    public Spawnable[] spawnables;
    [HideInInspector] public Dictionary<Vector2Int, Resource.CurrencyType> spawnedResources;

    public void Awake() { active = this; }

    public void GenerateWorldData()
    {
        // Create a new resource grid
        spawnedResources = new Dictionary<Vector2Int, Resource.CurrencyType>();

        // Set random seed
        Random.InitState(GameManager.seed.GetHashCode());

        // Iterate through offset values
        int startingRange = 0;
        foreach (Spawnable spawnable in spawnables)
            spawnable.spawnOffset = Random.Range(startingRange, startingRange += 10000);

        // Begin generating
        GenerateWorld();
    }


    // Loops through a new chunk and spawns resources based on perlin noise values
    private void GenerateWorld()
    {
        // Loop through x and y coordinates
        for (int x = -borderSize; x < borderSize; x++)
        {
            for (int y = -borderSize; y < borderSize; y++)
            {
                foreach (Spawnable spawnable in spawnables)
                {
                    // Calculate perlin noise pixel
                    float xCoord = ((float)x / spawnable.spawnScale) + spawnable.spawnOffset;
                    float yCoord = ((float)y / spawnable.spawnScale) + spawnable.spawnOffset;
                    float value = Mathf.PerlinNoise(xCoord, yCoord);

                    // If value exceeds threshold, try and generate
                    if (value >= spawnable.spawnThreshold)
                    {
                        TrySpawnResource(spawnable, x, y);
                        break;
                    }
                }
            }
        }
    }

    // Try and spawn a resource
    private void TrySpawnResource(Spawnable resource, int x, int y)
    {
        // Get x and y pos
        Vector2Int coords = new Vector2Int(x, y);

        // Check cell to make sure it's empty
        if (!spawnedResources.ContainsKey(coords) && CheckDistance(resource, coords))
        {
            // Create the resource
            resourceGrid.SetTile(new Vector3Int(coords.x, coords.y, 0), resource.tile);
            spawnedResources.Add(coords, resource.type);
        }
    }

    public bool CheckDistance(Spawnable resource, Vector2Int coords)
    {
        return (coords.x > resource.minSpawnDistance || coords.x < -resource.minSpawnDistance ||
               coords.y > resource.minSpawnDistance || coords.y < -resource.minSpawnDistance) &&
               coords.x < resource.maxSpawnDistance && coords.x > -resource.maxSpawnDistance &&
               coords.y < resource.maxSpawnDistance && coords.y > -resource.maxSpawnDistance;
    }

    // Check if a resource node exists
    public bool CheckNode(Vector2Int coords, Resource.CurrencyType type)
    {
        Vector2Int adjustedCoords = new Vector2Int(coords.x / 5, coords.y / 5);

        if (spawnedResources.ContainsKey(adjustedCoords) &&
            spawnedResources[adjustedCoords] == type) return true;
        else return false;
    }
}