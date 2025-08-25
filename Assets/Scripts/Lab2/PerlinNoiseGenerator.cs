using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public int depth;

    public int scale;

    private void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, DiamondSquare(width, 1f, 0.5f));

        return terrainData;
    }

    float[,] DiamondSquare(int size, float range, float roughness)
    {
        float[,] map = new float[size, size];

        //Initialize the map
        map[0, 0] = Random.value;
        map[0, size-1] = Random.value;
        map[size - 1, 0] = Random.value;
        map[size - 1, size - 1] = Random.value;

        void Divide(int x, int y, int step, float offset)
        {
            if (step < 2) return;
            int half = step / 2;

            float avg = (map[x, y] + map[x + step, y] + map[x, y + step] + map[x + step, y + step]) / 4; //Average

            map[x + half, y + half] = avg + (Random.value * 2f - 1f) * offset;

            //Setting the 4 midpoints

            // Top edge midpoint
            map[x + half, y] = (
                map[x, y] + map[x + step, y] + map[x + half, y + half]) / 3f + (Random.value * 2f - 1f);

            // Bottom edge midpoint
            map[x + half, y + step] = (
                map[x, y + step] + map[x + step, y + step] + map[x + half, y + half]) / 3f + (Random.value * 2f - 1f);

            // Left edge midpoint
            map[x, y + half] = (
                map[x, y] + map[x, y + step]+ map[x + half, y + half]) / 3f + (Random.value * 2f - 1f);

            // Right edge midpoint
            map[x + step, y + half] = (
                map[x + step, y] + map[x + step, y + step] + map[x + half, y + half]) / 3f + (Random.value * 2f - 1f);

            offset *= roughness;
            Divide(x, y, half, offset);
            Divide(x + half, y, half, offset);
            Divide(x, y + half, half, offset);
            Divide(x + half, y + half, half, offset);
        }

        Divide(0, 0, size - 1, range);

        float min = float.MaxValue, max = float.MinValue;
        foreach (float v in map) { if (v < min) min = v; if (v > max) max = v; }

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                map[i, j] = Mathf.InverseLerp(min, max, map[i, j]);

        return map; // Add this line to return the generated map
    }
}
