using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int levelSize;
    public TileBase groundTile;
    public Tilemap groundTileMap;
    int[,] level;
    public float scale;

    private void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        groundTileMap.ClearAllTiles();
        level = GenerateLevelArray(levelSize.x, levelSize.y, true);
        level = GenerateLevel(level);
        RenderLevel(level, groundTileMap, groundTile); 
    }

    private int[,] GenerateLevelArray(int x, int y, bool empty)
    {
        int[,] map = new int[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                map[i, j] = (empty) ? 0 : 1;
            }
        }
        return map;
    }

    private int[,] GenerateLevel(int[,] map)
    {
        float offsetX = UnityEngine.Random.Range(-100000f, 100000f);
        float offsetY = UnityEngine.Random.Range(-100000f, 100000f);

        for (int i = 0; i < levelSize.x; i++)
        {
            for (int j = 0; j < levelSize.y; j++)
            {
                map[i, j] = Mathf.RoundToInt(Mathf.PerlinNoise((i+offsetX)/scale, (j+offsetY)/scale));
                offsetY++;
            }
            offsetX++;
        }
        return map;
    }

    private void RenderLevel(int[,] map, Tilemap groundMap, TileBase groundBase)
    {
        for (int i = 0; i < levelSize.x; i++)
        {
            for (int j = 0; j < levelSize.y; j++)
            {
                if (map[i, j] == 1) groundMap.SetTile(new Vector3Int(i, j, 0), groundBase);
            }
        }
    }




    int CalculatePerlin(int x, int y)
    {
        // first get a raw perlin value
        float perlinRaw = Mathf.PerlinNoise(x, y);
        perlinRaw = Mathf.Clamp(perlinRaw, 0.0f, 1.0f);

        // translate that perlin value into the nearest int that can be used to spawn a tile
        float perlinMag = perlinRaw;

        int perlinProcessed = Mathf.FloorToInt(perlinMag);

        return perlinProcessed;
    }
}
