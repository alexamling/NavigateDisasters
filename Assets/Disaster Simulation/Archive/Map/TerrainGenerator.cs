using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class is used to generate a custom terrain mesh from a heightmap
/// Written by Alexander Amling
/// </summary>

public class TerrainGenerator : MonoBehaviour
{
    //private NavMeshSurface surface;

    [Range(0,250)]
    public float scale = 5;

    public int LOD = 32;
    private int spaceBetweenPoints = 1;
    private float worldToMapScale;

    #region Mesh Variables
    public TerrainData terrainData;
    
    public Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private new MeshCollider collider;

    private Vector3[] vertecies;
    private int[] triangles;
    private Vector2[] uvs;

    private int triangleIndex;

    public Texture2D heightMap;
    public int width;
    public int height;
    private int verteciesPerLine;
    #endregion

    public IEnumerator Load()
    {
        //surface = gameObject.AddComponent<NavMeshSurface>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        collider = gameObject.AddComponent<MeshCollider>();
        
        
        spaceBetweenPoints = (LOD == 0) ? 1 : LOD * 2;
        verteciesPerLine = ((width - 1) / spaceBetweenPoints) + 1;

        worldToMapScale = (float)heightMap.width / width;

        vertecies = new Vector3[verteciesPerLine * verteciesPerLine];
        uvs = new Vector2[verteciesPerLine * verteciesPerLine];
        triangles = new int[(verteciesPerLine - 1) * (verteciesPerLine - 1) * 6];

        yield return StartCoroutine(GenerateMesh());
    }

    void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    IEnumerator GenerateMesh()
    {
        // generate the terrain
        terrainData.heightmapResolution = width;
        float[,] heights = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                heights[x, y] = heightMap.GetPixel(
                        Mathf.FloorToInt(y * worldToMapScale),
                        Mathf.FloorToInt(x * worldToMapScale)
                        ).grayscale + Random.Range(-0.01f, .01f);
            }
            if(y%2 == 0)
                yield return null;
        }
        
        terrainData.SetHeights(0, 0, heights);

        int index = 0;
        for (int y = 0; y < height; y +=  spaceBetweenPoints)
        {
            for (int x = 0; x < width; x += spaceBetweenPoints)
            {

                vertecies[index] = new Vector3(
                    (-x + (width * .5f)) / (worldToMapScale * 2), 
                    heightMap.GetPixel(
                        Mathf.FloorToInt(x * worldToMapScale), 
                        Mathf.FloorToInt(y * worldToMapScale)
                        ).grayscale * scale + Random.Range(.0f,.01f),
                    (y - (height * .5f)) / (worldToMapScale * 2)
                );

                uvs[index] = new Vector2((float)x / width, (float)y / height);

                if (x < width - spaceBetweenPoints && y < height - spaceBetweenPoints)
                {
                    AddTriangle(index, index + verteciesPerLine + 1, index + verteciesPerLine);
                    AddTriangle(index + verteciesPerLine + 1, index, index + 1);
                }
                index++;

                if(Time.frameCount % 256 == 0)
                    yield return null;
            }
            yield return null;
        }


        mesh = new Mesh();
        mesh.vertices = vertecies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        //meshFilter.mesh = mesh;
        //collider.sharedMesh = mesh;

        //surface.BuildNavMesh();
    }
}
