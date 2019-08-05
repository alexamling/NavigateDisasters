using UnityEngine;
using System.Collections;

public class UnderwaterPostEffects : MonoBehaviour
{
    public Color FogColor = new Color(87f / 255f, 190f / 255f, 219f / 255f, 1);
    public float FogDensity = 0.05f;
    public bool UseSunShafts = true;
    public float ShuftsIntensity = 5f;
    public WFX_SunShafts.ShaftsScreenBlendMode SunShuftsScreenBlend = WFX_SunShafts.ShaftsScreenBlendMode.Screen;


    private Vector3 SunShaftTargetPosition = new Vector3(0, 7, 10);
    private Camera cam;
    private WFX_SunShafts SunShafts;

    void OnEnable()
    {
        cam = Camera.main;
        SunShafts = cam.gameObject.AddComponent<WFX_SunShafts> ();
        SunShafts.sunShaftIntensity = ShuftsIntensity;
        var target = new GameObject("SunShaftTarget");
        SunShafts.sunTransform = target.transform;
        target.transform.parent = cam.transform;
        target.transform.localPosition = SunShaftTargetPosition;
        SunShafts.screenBlendMode = SunShuftsScreenBlend;
        SunShafts.sunShaftsShader = Shader.Find("Hidden/SunShaftsComposite");
        SunShafts.simpleClearShader = Shader.Find("Hidden/SimpleClear");

        var underwater = cam.gameObject.AddComponent<Underwater>();
        underwater.UnderwaterLevel = transform.position.y;
        underwater.FogColor = FogColor;
        underwater.FogDensity = FogDensity;
    }


    // Update is called once per frame
    void Update()
    {
        if (cam == null)
            return;
        if (cam.transform.position.y < transform.position.y)
        {
            if (!SunShafts.enabled)
                SunShafts.enabled = true;
        }
        else if (SunShafts.enabled)
            SunShafts.enabled = false;
    }
}