using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCarLights : MonoBehaviour
{
    public float interval = 0.125f;
    public Texture redLightEmissiveMap;
    public Texture blueLightEmissiveMap;
    public Light redLight;
    public Light blueLight;
    
    private Material material;

	void Start ()
    {
        material = GetComponentInChildren<Renderer>().material;
        redLight = transform.GetChild(3).GetComponent<Light>();
        blueLight = transform.GetChild(4).GetComponent<Light>();

        material.SetTexture("_EmissionMap", redLightEmissiveMap);
        redLight.enabled = true;
        blueLight.enabled = false;
    }

    void Update()
    {
        if ((Time.time % (interval * 2)) < interval)
        {
            material.SetTexture("_EmissionMap", redLightEmissiveMap);
            redLight.enabled = true;
            blueLight.enabled = false;
        }
        else
        {
            material.SetTexture("_EmissionMap", blueLightEmissiveMap);
            redLight.enabled = false;
            blueLight.enabled = true;
        }
	}
}
