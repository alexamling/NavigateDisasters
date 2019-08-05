using UnityEngine;
using System.Collections;

public class Underwater : MonoBehaviour
{

    //This script enables underwater effects. Attach to main camera.

    //Define variable
    public float UnderwaterLevel = 0;
    public Color FogColor = new Color(0, 0.4f, 0.7f, 1);
    public float FogDensity = 0.04f;
    public FogMode FogMode = FogMode.Exponential;

    //The scene's default fog settings
    private bool defaultFog;
    private Color defaultFogColor;
    private float defaultFogDensity;
    private FogMode defaultFogMod;
    private Material defaultSkybox;

    private void Start()
    {
        defaultFog = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        defaultFogMod = RenderSettings.fogMode;
    }

    void Update()
    {
        defaultFogDensity = RenderSettings.fogDensity;
        if (transform.position.y < UnderwaterLevel)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogDensity = FogDensity;
            RenderSettings.fogMode = FogMode;
        }
        else
        {
            RenderSettings.fog = defaultFog;
            RenderSettings.fogColor = defaultFogColor;
            RenderSettings.fogDensity = defaultFogDensity;
            RenderSettings.fogMode = defaultFogMod;
            RenderSettings.fogStartDistance = -300;
        }
    }
}