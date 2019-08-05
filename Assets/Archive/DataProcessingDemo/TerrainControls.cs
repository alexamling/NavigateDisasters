using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainControls : MonoBehaviour
{
    public ComputeShader shader;
    public Texture2D explosionEffect;
    private int kernel;

    public Terrain terrain;
    private TerrainData terrainData;

    public List<Vector4> explosionLocations;

    private float[] locationArray;
    private int numImpacts;

    public RenderTexture heightMap;

    // Start is called before the first frame update
    void Start()
    {
        terrainData = terrain.terrainData;
        locationArray = new float[128];

        UpdateLocationArray();

        kernel = shader.FindKernel("CSMain");

        heightMap = new RenderTexture(terrainData.heightmapWidth, terrainData.heightmapHeight, 24);
        heightMap.enableRandomWrite = true;
        heightMap.Create();

        shader.SetTexture(kernel, "Result", heightMap);
        shader.SetTexture(kernel, "ExplosionPattern", explosionEffect);
        shader.Dispatch(kernel, terrainData.heightmapWidth / 8, terrainData.heightmapHeight / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLocationArray();

        shader.SetFloats("ExplosionData", locationArray);
        shader.SetInt("NumImpacts", numImpacts);

        shader.Dispatch(kernel, terrainData.heightmapWidth / 8, terrainData.heightmapHeight / 8, 1);
        EditTerrain();
    }

    private void EditTerrain()
    {
        int heightmapWidth = terrainData.heightmapWidth;
        int heightmapHeight = terrainData.heightmapHeight;

        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        Texture2D data = new Texture2D(heightMap.width, heightMap.height, TextureFormat.RGBAFloat, false);

        RenderTexture.active = heightMap;
        data.ReadPixels(new Rect(0, 0, heightMap.width, heightMap.height), 0, 0);
        data.Apply();
        RenderTexture.active = null;

        Color[] colors = data.GetPixels();

        for(int i = 0; i < colors.Length; i++)
        {
            heights[i / heightMap.height, i % heightMap.width] = colors[i].r;
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void UpdateLocationArray()
    {
        numImpacts = explosionLocations.Count;

        for(int i = 0; i < numImpacts; i++)
        {
            locationArray[i * 4] = Mathf.FloorToInt(explosionLocations[i].x);        // x pos
            locationArray[i * 4 + 1] = Mathf.FloorToInt(explosionLocations[i].y);    // y pos
            locationArray[i * 4 + 2] = 1 / explosionLocations[i].z;                  // scale
            locationArray[i * 4 + 3] = explosionLocations[i].w;                      // intensity
        }
    }
}
