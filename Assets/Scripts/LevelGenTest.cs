using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

public class LevelGenTest : MonoBehaviour
{
    [Header("Terrain Elements")]
    [SerializeField] private Tilemap groundTileMap;
    [Tooltip("The ground tiles to terrain generation. Second element will be the infill"), SerializeField] private TileBase[] groundTiles;
    [Tooltip("Controls the density of terrain"), SerializeField] private float perlinMagnification;
    private int[,] terrain;


    [Header("Map Elements")]
    [Tooltip("Size of the map"), SerializeField] private Vector2Int mapSize;

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
        
        // generate an intial grid based on perlin noise
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                terrain[i, j] = CalculatePerlin(i + offsetX, j + offsetY);
            }
        }

        // adjust the grid values by adding the above value to each grid cell
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = mapSize.y-1; j > 0; j--)
            {
                terrain[i, j] += terrain[i, j - 1];
                if (terrain[i, j] > groundTiles.Length) terrain[i, j] = groundTiles.Length;
            }
        }
    }

    // places tiles within the tilemap according to the given terrain array
    void RenderLevel()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (terrain[i,j] > 0)
                {
                    groundTileMap.SetTile(new Vector3Int(i, j, 0), groundTiles[ terrain[i, j]-1 ]);
                }
            }
        }
    }
    /*
     * An old version of the CalculatePerlin function. No longer used, but kept for reference.
    int CalculatePerlin(int x, int y, float xOffset, float yOffset)
    {
        // get a raw value from the perlin noise graph
        float rawPerlin = Mathf.PerlinNoise((x + xOffset) / perlinMagnification, (y + yOffset) / perlinMagnification);

        // clamp that raw value as sometimes the Mathf function gives an unexpected value
        rawPerlin = Mathf.Clamp(rawPerlin, 0.0f, 1.0f);

        // scale the raw value by the amount of available tiles
        float perlinScaled = rawPerlin * groundTiles.Length + airMultiplier;

        // floor the result to an integer
        int perlinMagnitude = Mathf.FloorToInt(perlinScaled);

        // return the result
        return perlinMagnitude;
    }
    */
    int CalculatePerlin(int x, int y)
    {
        // get a raw perlin value, clamping it to account to rare edge cases
        float perlinRaw = Mathf.PerlinNoise(x / perlinMagnification, y / perlinMagnification);
        float perlinClamped = Mathf.Clamp01(perlinRaw);

        // round the raw value to get either 0 or 1;
        int perlinMagnitude = Mathf.RoundToInt(perlinClamped);

        return perlinMagnitude;
    }

    void DisplayArray()
    {
        string val = String.Empty;
        for (int i = 0; i < mapSize.y; i++)
        {
            val = String.Empty;
            for (int j = 0; j < mapSize.x; j++)
            {
                val += terrain[j, i].ToString() + " ";
            }
            Debug.Log(val);

        }
    }
}
