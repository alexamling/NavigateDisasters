using System;
using UnityEngine;
using System.Collections;

public class DemoGUIWater : MonoBehaviour
{
    public float UpdateInterval = 0.5F;
    public int MaxScenes = 2;

    public bool IsMobileScene;
    public Light Sun;
    public GameObject SunTransform;
    public GameObject Boat;
    public GameObject water1;
    public GameObject water2;
    public float angle = 130;

    private bool canUpdateTestMaterial;
    private GameObject cam;
    private GUIStyle guiStyleHeader = new GUIStyle();
    private Material currentWaterMaterial, causticMaterial;
    private GameObject currentWater;
    private float transparent, fadeBlend, refl, refraction;
    private float waterWaveScaleXZ = 1;
    private Vector4 waterDirection, causticDirection, foamDirection, ABDirection, CDDirection;
    private float direction = 1;
    private Color reflectionColor;
    private Vector3 oldCausticScale;
    private float oldTextureScale, oldWaveScale;
    private GameObject caustic;
    private float startSunIntencity;


    private void Start()
    {
        guiStyleHeader.fontSize = 18;
        guiStyleHeader.normal.textColor = new Color(1, 0, 0);
        UpdateCurrentWater();
    }

    private void UpdateCurrentWater()
    {
        if (Boat != null)
        {
            Boat.SetActive(false);
            Boat.SetActive(true);
        }
        startSunIntencity = Sun.intensity;
        currentWater = GameObject.Find("Water");
        currentWaterMaterial = currentWater.GetComponent<Renderer>().material;

        refl = currentWaterMaterial.GetColor("_ReflectionColor").r;
        if (!IsMobileScene) transparent = currentWaterMaterial.GetFloat("_DepthTransperent");
        if (!IsMobileScene) fadeBlend = currentWaterMaterial.GetFloat("_FadeDepth");
        refraction = currentWaterMaterial.GetFloat("_Distortion");
        oldTextureScale = currentWaterMaterial.GetFloat("_TexturesScale");
        oldWaveScale = currentWaterMaterial.GetFloat("_WaveScale");
        var infiniteMesh = GameObject.Find("InfiniteWaterMesh");
        if (infiniteMesh != null) infiniteMesh.GetComponent<Renderer>().material = currentWaterMaterial;
        var projectorCausticScale = GameObject.Find("ProjectorCausticScale");
        if (projectorCausticScale != null) oldCausticScale = projectorCausticScale.transform.localScale;

        caustic = GameObject.Find("Caustic");
        if (IsMobileScene) caustic.SetActive(true);

        if (!IsMobileScene) causticMaterial = caustic.GetComponent<Projector>().material;
        waterDirection = currentWaterMaterial.GetVector("_Direction");
        if (!IsMobileScene) foamDirection = currentWaterMaterial.GetVector("_FoamDirection");
        if (!IsMobileScene) causticDirection = causticMaterial.GetVector("_CausticDirection");
        ABDirection = currentWaterMaterial.GetVector("_GDirectionAB");
        CDDirection = currentWaterMaterial.GetVector("_GDirectionCD");
    }

    private void OnGUI()
    {
        if (IsMobileScene)
            GUIMobile();
        else
            GUIPC();
    }

    private void GUIMobile()
    {
        if (currentWaterMaterial == null)
            return;

        if (GUI.Button(new Rect(10, 35, 150, 40), "On/Off Ripples"))
        {
            caustic.SetActive(true);
            water1.SetActive(!water1.activeSelf);
            water2.SetActive(!water2.activeSelf);
            caustic = GameObject.Find("Caustic");
            if (IsMobileScene)
                caustic.SetActive(true);
        }

        if (GUI.Button(new Rect(10, 190, 150, 40), "On/Off caustic"))
        {
            caustic.SetActive(!caustic.activeSelf);
        }
        var labelTexColor = new GUIStyle();
        labelTexColor.normal.textColor = new Color(1, 1, 1);

        angle = GUI.HorizontalSlider(new Rect(10, 102, 120, 15), angle, 30, 240);
        GUI.Label(new Rect(140, 100, 30, 30), "Day Time", labelTexColor);
        var intensity = Mathf.Sin((angle - 60) / 50);
        Sun.intensity = Mathf.Clamp01(intensity) * startSunIntencity + 0.05f;
        SunTransform.transform.rotation = Quaternion.Euler(0, 0, angle);


        refl = GUI.HorizontalSlider(new Rect(10, 122, 120, 15), refl, 0, 1);
        reflectionColor = new Color(refl, refl, refl, 1);
        GUI.Label(new Rect(140, 120, 30, 30), "Reflection", labelTexColor);
        currentWaterMaterial.SetColor("_ReflectionColor", reflectionColor);

        refraction = GUI.HorizontalSlider(new Rect(10, 142, 120, 15), refraction, 0, 700);
        GUI.Label(new Rect(140, 140, 30, 30), "Distortion", labelTexColor);
        currentWaterMaterial.SetFloat("_Distortion", refraction);

        waterWaveScaleXZ = GUI.HorizontalSlider(new Rect(10, 162, 120, 15), waterWaveScaleXZ, 0.3f, 3);
        GUI.Label(new Rect(140, 160, 30, 30), "Scale", labelTexColor);

        //projectorScale.transform.localScale = new Vector3(waterWaveScaleXZ * oldProjectorScale.x, waterWaveScaleXZ * oldProjectorScale.y, waterWaveScaleXZ * oldProjectorScale.z);
        currentWaterMaterial.SetFloat("_WaveScale", oldWaveScale * waterWaveScaleXZ);
        currentWaterMaterial.SetFloat("_TexturesScale", oldTextureScale * waterWaveScaleXZ);

    }

    void GUIPC()
    {
        if (currentWaterMaterial == null)
            return;

        if (GUI.Button(new Rect(10, 35, 150, 40), "Change Scene "))
        {
            if (Application.loadedLevel == MaxScenes - 1)
                Application.LoadLevel(0);
            else
                Application.LoadLevel(Application.loadedLevel + 1);
            UpdateCurrentWater();
        }

        var labelTexColor = new GUIStyle();
        labelTexColor.normal.textColor = new Color(1, 1, 1);

        angle = GUI.HorizontalSlider(new Rect(10, 102, 120, 15), angle, 30, 240);
        GUI.Label(new Rect(140, 100, 30, 30), "Day Time", labelTexColor);
        var intensity = Mathf.Sin((angle - 60) / 50);
        Sun.intensity = Mathf.Clamp01(intensity) * startSunIntencity + 0.05f;
        SunTransform.transform.rotation = Quaternion.Euler(0, 0, angle);

        transparent = GUI.HorizontalSlider(new Rect(10, 122, 120, 15), transparent, 0, 1);
        GUI.Label(new Rect(140, 120, 30, 30), "Depth Transperent", labelTexColor);
        currentWaterMaterial.SetFloat("_DepthTransperent", transparent);

        fadeBlend = GUI.HorizontalSlider(new Rect(10, 142, 120, 15), fadeBlend, 0, 1);
        GUI.Label(new Rect(140, 140, 30, 30), "Fade Depth", labelTexColor);
        currentWaterMaterial.SetFloat("_FadeDepth", fadeBlend);

        refl = GUI.HorizontalSlider(new Rect(10, 162, 120, 15), refl, 0, 1);
        reflectionColor = new Color(refl, refl, refl, 1);
        GUI.Label(new Rect(140, 160, 30, 30), "Reflection", labelTexColor);
        currentWaterMaterial.SetColor("_ReflectionColor", reflectionColor);

        refraction = GUI.HorizontalSlider(new Rect(10, 182, 120, 15), refraction, 0, 700);
        GUI.Label(new Rect(140, 180, 30, 30), "Distortion", labelTexColor);
        currentWaterMaterial.SetFloat("_Distortion", refraction);

        waterWaveScaleXZ = GUI.HorizontalSlider(new Rect(10, 202, 120, 15), waterWaveScaleXZ, 0.3f, 3);
        GUI.Label(new Rect(140, 200, 30, 30), "Scale", labelTexColor);

        //projectorScale.transform.localScale = new Vector3(waterWaveScaleXZ * oldProjectorScale.x, waterWaveScaleXZ * oldProjectorScale.y, waterWaveScaleXZ * oldProjectorScale.z);
        currentWaterMaterial.SetFloat("_WaveScale", oldWaveScale * waterWaveScaleXZ);
        currentWaterMaterial.SetFloat("_TexturesScale", oldTextureScale * waterWaveScaleXZ);

        var projectorCausticScale = GameObject.Find("ProjectorCausticScale");
        var currentCausticScale = oldCausticScale * waterWaveScaleXZ;
        if ((projectorCausticScale.transform.localScale - currentCausticScale).magnitude > 0.01)
        {
            projectorCausticScale.transform.localScale = currentCausticScale;
            caustic.GetComponent<ProjectorMatrix>().UpdateMatrix();
        }

        //Water Direction
        direction = GUI.HorizontalSlider(new Rect(10, 222, 120, 15), direction, 1, -1);
        GUI.Label(new Rect(140, 220, 30, 30), "Direction", labelTexColor);
        currentWaterMaterial.SetVector("_Direction", waterDirection * direction);
        currentWaterMaterial.SetVector("_FoamDirection", foamDirection * direction);
        causticMaterial.SetVector("_CausticDirection", causticDirection * direction);
        currentWaterMaterial.SetVector("_GDirectionAB", ABDirection * direction);
        currentWaterMaterial.SetVector("_GDirectionCD", CDDirection * direction);
    }

    private void OnDestroy()
    {
        if (!IsMobileScene) causticMaterial.SetVector("_CausticDirection", causticDirection);
    }

    private void OnSetColorMain(Color color)
    {
        currentWaterMaterial.SetColor("_Color", color);
    }

    //private void OnGetColorMain(ColorPicker picker)
    //{
    //    if (picker != null && currentWaterMaterial != null)
    //        picker.NotifyColor(currentWaterMaterial.GetColor("_Color"));
    //}

    //private void OnSetColorFade(Color color)
    //{
    //    currentWaterMaterial.SetColor("_FadeColor", color);
    //}

    //private void OnGetColorFade(ColorPicker picker)
    //{
    //    if (picker != null && currentWaterMaterial != null)
    //        picker.NotifyColor(currentWaterMaterial.GetColor("_FadeColor"));
    //}

}