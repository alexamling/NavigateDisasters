using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    [Range(0, 250)]
    public float scale = 5;

    public int LOD = 32;
    private int spaceBetweenPoints = 1;
    private float worldToMapScale;

    #region Mesh Variables
    public Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private new MeshCollider collider;

    private Vector3[] vertecies;
    private int[] triangles;
    private Vector2[] uvs;

    private int triangleIndex;

    public Texture2D heightMap;
    public Vector2 offset;
    public int width;
    public int height;
    private int verteciesPerLine;
    #endregion


    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
