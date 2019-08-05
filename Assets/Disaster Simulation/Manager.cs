using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class that allows for more flexible use and implementation of managers as needed
/// </summary>

public abstract class Manager : MonoBehaviour
{
    [Range(64, 8192)]
    public int mapWidth = 4096;
    [Range(64, 8192)]
    public int mapHeight = 4096;

    public Texture2D heightMap;

    public RenderTexture output;

    public bool isLoaded;

    public abstract IEnumerator Load();
}
