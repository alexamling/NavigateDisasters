using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamableObject : MonoBehaviour
{
    public bool isOnFire = false;
    public float burnTime = 12f;
    public float burnProgress;
    public Vector2 position;
    public float Temp {
        get
        {
            float value = (burnProgress / (burnTime * .5f)) -1;
            value *= value;
            value += 1;
            return value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnFire)
        {
            burnProgress += Time.deltaTime;
        }
    }
}
