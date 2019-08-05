using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controlls the water level of flooding
/// </summary>

public class FloodManager : Manager
{
    public GameObject waterObject;

    public float waterLevel;
    public float maxHeight = 50;
    public float baseHeight;

    public AnimationCurve floodCurve;

    public Material mat;

    [Header("Shader Variables")]
    #region Shader Variables
    public Color[] colors;
    public float[] colorStartHeights;
    #endregion

    void Start()
    {
        output = new RenderTexture(mapWidth, mapHeight, 24);
        output.enableRandomWrite = true;
        output.Create();
    }
    
    void Update()
    {
        waterLevel = Time.time;
        waterLevel = floodCurve.Evaluate(waterLevel / 100.0f) * maxHeight;
        waterObject.transform.position = new Vector3(0, waterLevel * .5f + baseHeight, 0);
        waterObject.transform.localScale = new Vector3(mapWidth, waterLevel, mapHeight);
    }

    public override IEnumerator Load()
    {
        yield return null;
    }
}
