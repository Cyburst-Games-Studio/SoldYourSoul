using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

public class LevelGenTest : MonoBehaviour
{
    [Header("Terrain Elements")]
    [SerializeField] private Tilemap groundTileMap;
    [SerializeField] private TileBase[] groundTiles;
    [SerializeField] private float perlinMagnification;
    private int[,] terrain;

    [Header("Map Elements")]
    [SerializeField] private Vector2Int mapSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // clear any tiles in the level
        groundTileMap.ClearAllTiles();

        // intialize the terrain array to be used to store terrain data for later
        terrain = InitializeArray();

        // populate the array with values using GenerateTerrain function
        GenerateTerrain();

        // render the terrain using RenderLevel
        RenderLevel();
    }

    void OnJump()
    {
        Start();
    }

    int[,] InitializeArray()
    {
        int[,] arr = new int[mapSize.x,mapSize.y];

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                arr[i, j] = 0;
            }
        }

        return arr;
    }

    // place values into the terrain array using the CalculatePerlin() function below
    void GenerateTerrain()
    {
        int offsetX = UnityEngine.Random.Range(-100000, 100000);
        int offsetY = UnityEngine.Random.Range(-100000, 100000);
        
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                terrain[i, j] = CalculatePerlin(i, j, offsetX, offsetY);
                offsetY++;
            }
            offsetX++;
        }
    }

    // places tiles within the tilemap according to the given terrain array
    void RenderLevel()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (terrain[i, j] > 0)
                {
                    groundTileMap.SetTile(new Vector3Int(i, j, 0), groundTiles[ terrain[i, j] ]);
                }
            }
        }
    }

    int CalculatePerlin(int x, int y, float xOffset, float yOffset)
    {
        // get a raw value from the perlin noise graph
        float rawPerlin = Mathf.PerlinNoise((x + xOffset) / perlinMagnification, (y + yOffset) / perlinMagnification);

        // clamp that raw value as sometimes the Mathf function gives an unexpected value
        rawPerlin = Mathf.Clamp(rawPerlin, 0.0f, 1.0f);

        // scale the raw value by the amount of available tiles
        float perlinScaled = rawPerlin * groundTiles.Length + 1;

        // floor the result to an integer
        int perlinMagnitude = Mathf.FloorToInt(perlinScaled);

        // return the result
        return perlinMagnitude;
    }
}
