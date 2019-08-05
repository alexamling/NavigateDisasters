using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class WaterRipples : MonoBehaviour
{
    [Range(20, 200)]
    public int UpdateFPS = 30;

    public bool Multithreading = true;
    public int DisplacementResolution = 128;
    public float Damping = 0.005f;

    [Range(0.0001f, 2)]
    public float Speed = 1.5f;

    public bool UseSmoothWaves;
    public bool UseProjectedWaves;
    public Texture2D CutOutTexture;

    private Transform t;
    private float textureColorMultiplier = 10;
    private Texture2D displacementTexture;
    private Vector2[,] waveAcceleration;
    private Color[] col;
    private Vector3[] wavePoints;
    private Vector3 scaleBounds;
    private float inversedDamping;
    private float[] cutOutTextureGray;
    private bool cutOutTextureInitialized;

    private Thread thread;
    private bool canUpdate = true;
    private double threadDeltaTime;
    private DateTime oldDateTime;
    private Vector2 movedObjPos, projectorPosition;


    private Vector4 _GAmplitude;
    private Vector4 _GFrequency;
    private Vector4 _GSteepness;
    private Vector4 _GSpeed;
    private Vector4 _GDirectionAB;
    private Vector4 _GDirectionCD;


    private void OnEnable()
    {
        canUpdate = true;
       
        Shader.EnableKeyword("ripples_on");
        var rend = GetComponent<Renderer>();
        _GAmplitude = rend.sharedMaterial.GetVector("_GAmplitude");
        _GFrequency = rend.sharedMaterial.GetVector("_GFrequency");
        _GSteepness = rend.sharedMaterial.GetVector("_GSteepness");
        _GSpeed = rend.sharedMaterial.GetVector("_GSpeed");
        _GDirectionAB = rend.sharedMaterial.GetVector("_GDirectionAB");
        _GDirectionCD = rend.sharedMaterial.GetVector("_GDirectionCD");

        t = transform;
        scaleBounds = GetComponent<MeshRenderer>().bounds.size;
        InitializeRipple();

        if (Multithreading)
        {
            thread = new Thread(UpdateRipples);
            thread.Start();
        }
    }

    /// <summary>
    /// Allow you get wave position in world space (with ripples + gerstner waves). 
    /// </summary>
    /// <param name="position">Default position of wave</param>
    /// <returns>World position with wave offset</returns>
    public Vector3 GetOffsetByPosition(Vector3 position)
    {
        var pos = GerstnerOffset4(new Vector2(position.x, position.z), _GSteepness, _GAmplitude, _GFrequency, _GSpeed, _GDirectionAB, _GDirectionCD);
        pos.y += GetTextureHeightByPosition(position.x, position.z);
        pos.y += t.position.y;
        return pos;
    }

    /// <summary>
    /// Allow you create point ripple in the water. 
    /// </summary>
    /// <param name="position">World position inside the water mesh</param>
    /// <param name="velocity">Ripple velocity. Can be positive and negative (above or below the water level). </param>
    public void CreateRippleByPosition(Vector3 position, float velocity)
    {
        position.x += scaleBounds.x / 2 - t.position.x;
        position.z += scaleBounds.z / 2 - t.position.z;
        position.x /= scaleBounds.x;
        position.z /= scaleBounds.z;
        position.x *= DisplacementResolution;
        position.z *= DisplacementResolution;
        SetRippleTexture((int)position.x, (int)position.z, velocity);
    }

    private void InitializeRipple()
    {
        inversedDamping = 1f - Damping;
        displacementTexture = new Texture2D(DisplacementResolution, DisplacementResolution, TextureFormat.RGBA32, false);
        displacementTexture.wrapMode = TextureWrapMode.Clamp;
        displacementTexture.filterMode = FilterMode.Bilinear;
        Shader.SetGlobalTexture("_WaterDisplacementTexture", displacementTexture);
        wavePoints = new Vector3[DisplacementResolution * DisplacementResolution];
        col = new Color[DisplacementResolution * DisplacementResolution];
        waveAcceleration = new Vector2[DisplacementResolution, DisplacementResolution];

        for (int i = 0; i < DisplacementResolution * DisplacementResolution; i++)
        {
            col[i] = new Color(0f, 0f, 0f);
            wavePoints[i] = new Vector3(0f, 0f);
        }

        for (int i = 0; i < DisplacementResolution; i++)
        {
            for (int j = 0; j < DisplacementResolution; j++)
            {
                waveAcceleration[i, j] = new Vector2(0f, 0f);
            }
        }
        if (CutOutTexture != null)
        {
            var scaledCutOutTexture = ScaleTexture(CutOutTexture, DisplacementResolution, DisplacementResolution);
            var colors = scaledCutOutTexture.GetPixels();
            cutOutTextureGray = new float[DisplacementResolution * DisplacementResolution];
            for (int i = 0; i < colors.Length; i++)
            {
                cutOutTextureGray[i] = colors[i].r * 0.299f + colors[i].g * 0.587f + colors[i].b * 0.114f;
            }
            cutOutTextureInitialized = true;
        }
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
        var pixels = source.GetPixels();
        result.SetPixels(pixels);
        TextureScale.Bilinear(result, targetWidth, targetHeight);
        result.Apply();
        return result;
    }

    private void UpdateRipples()
    {
        oldDateTime = DateTime.UtcNow;
        while (canUpdate)
        {
            threadDeltaTime = (DateTime.UtcNow - oldDateTime).TotalMilliseconds / 1000;
            oldDateTime = DateTime.UtcNow;
            var sleepTime = (int)(1000f / UpdateFPS - threadDeltaTime);
            if (sleepTime > 0)
                Thread.Sleep(sleepTime);
            RippleTextureRecalculate();
        }
    }

    private void FixedUpdate()
    {
        if (!Multithreading)
            RippleTextureRecalculate();
        displacementTexture.SetPixels(col);
        displacementTexture.Apply(false);
    }

    private void Update()
    {
        movedObjPos = new Vector2(t.position.x, t.position.z);

    }

    private void UpdateProjector()
    {
        //relative position offset for array of pixels.
        var xOffset = (int)(DisplacementResolution * movedObjPos.x / scaleBounds.x - projectorPosition.x);
        var yOffset = (int)(DisplacementResolution * movedObjPos.y / scaleBounds.z - projectorPosition.y);
        projectorPosition.x += xOffset;
        projectorPosition.y += yOffset;

        if (xOffset == 0 && yOffset == 0)
            return;

        //it worked every frame (for exmaple with 60 fps and grid 128*128 it will be about 1 million iterations. 
        //So i can't use method with overhead. And i can't use this code on gpu, because code can't be paralleled :( 
        if (xOffset >= 0 && yOffset >= 0)
            for (int i = 1; i < DisplacementResolution; i++)
            {
                for (int j = 0; j < DisplacementResolution; j++)
                {
                    if (i + yOffset > 0 && i + yOffset < DisplacementResolution && j + xOffset > 0 && j + xOffset < DisplacementResolution)
                    {
                        waveAcceleration[j, i] = waveAcceleration[j + xOffset, i + yOffset];
                        wavePoints[j + (i * DisplacementResolution)] = wavePoints[j + xOffset + ((i + yOffset) * DisplacementResolution)];
                    }
                }
            }

        if (xOffset >= 0 && yOffset < 0)
            for (int i = DisplacementResolution - 1; i >= 0; i--)
            {
                for (int j = 0; j < DisplacementResolution; j++)
                {
                    if (i + yOffset > 0 && i + yOffset < DisplacementResolution && j + xOffset > 0 && j + xOffset < DisplacementResolution)
                    {
                        waveAcceleration[j, i] = waveAcceleration[j + xOffset, i + yOffset];
                        wavePoints[j + (i * DisplacementResolution)] = wavePoints[j + xOffset + ((i + yOffset) * DisplacementResolution)];
                    }
                }
            }

        if (xOffset < 0 && yOffset >= 0)
            for (int i = 0; i < DisplacementResolution; i++)
            {
                for (int j = DisplacementResolution - 1; j >= 0; j--)
                {
                    if (i + yOffset > 0 && i + yOffset < DisplacementResolution && j + xOffset > 0 && j + xOffset < DisplacementResolution)
                    {
                        waveAcceleration[j, i] = waveAcceleration[j + xOffset, i + yOffset];
                        wavePoints[j + (i * DisplacementResolution)] = wavePoints[j + xOffset + ((i + yOffset) * DisplacementResolution)];
                    }
                }
            }

        if (xOffset < 0 && yOffset < 0)
            for (int i = DisplacementResolution - 1; i >= 0; i--)
            {
                for (int j = DisplacementResolution - 1; j >= 0; j--)
                {
                    if (i + yOffset > 0 && i + yOffset < DisplacementResolution && j + xOffset > 0 && j + xOffset < DisplacementResolution)
                    {
                        waveAcceleration[j, i] = waveAcceleration[j + xOffset, i + yOffset];
                        wavePoints[j + (i * DisplacementResolution)] = wavePoints[j + xOffset + ((i + yOffset) * DisplacementResolution)];
                    }
                }
            }

        //need to clear fringe of array. Because ripples generating should have zero-fringes.
        var defaultVector = Vector2.zero;
        for (int i = 0; i < DisplacementResolution; i++)
        {
            waveAcceleration[0, i] = defaultVector;
            wavePoints[(i * DisplacementResolution)] = defaultVector;
            waveAcceleration[DisplacementResolution - 1, i] = defaultVector;
            wavePoints[DisplacementResolution - 1 + (i * DisplacementResolution)] = defaultVector;

            waveAcceleration[i, 0] = defaultVector;
            wavePoints[i] = defaultVector;
            waveAcceleration[i, DisplacementResolution - 1] = defaultVector;
            wavePoints[DisplacementResolution - 1 + i] = defaultVector;
        }
    }

    private void OnDestroy()
    {
        canUpdate = false;
    }

    private void OnDisable()
    {
        
        Shader.DisableKeyword("ripples_on");
        canUpdate = false;
    }



    private void RippleTextureRecalculate()
    {
        if (UseProjectedWaves)
            UpdateProjector();
        var length = wavePoints.Length;
        var res1 = DisplacementResolution + 1;
        var resMinus1 = DisplacementResolution - 2;
        var lRes1 = length - (DisplacementResolution + 1);
        int x, y;
        float tempSmooth;
        float str;

        //ripples generaion
        for (int i = 0; i < length; ++i)
        {
            if (i >= res1 && i < lRes1 && i % DisplacementResolution > 0)
            {
                x = i % DisplacementResolution;
                y = i / DisplacementResolution;
                tempSmooth = (wavePoints[i - 1].y + wavePoints[i + 1].y + wavePoints[i - DisplacementResolution].y + wavePoints[(i + DisplacementResolution)].y) / 4;
                waveAcceleration[x, y].y += (tempSmooth - waveAcceleration[x, y].x);
            }
        }

        var currentSpeed = Speed;
        if (!Multithreading)
            currentSpeed *= Time.fixedDeltaTime * UpdateFPS;
        for (int i = 0; i < DisplacementResolution; i++)
        {
            for (int j = 0; j < DisplacementResolution; j++)
            {
                waveAcceleration[j, i].x += waveAcceleration[j, i].y * currentSpeed;

                if (cutOutTextureInitialized)
                    waveAcceleration[j, i].x *= cutOutTextureGray[j + (i * DisplacementResolution)];

                waveAcceleration[j, i].y *= inversedDamping;
                waveAcceleration[j, i].x *= inversedDamping;
                wavePoints[j + (i * DisplacementResolution)].y = waveAcceleration[j, i].x;
                if (!UseSmoothWaves)
                {
                    //col[j + (i * DisplacementResolution)].r = waveAcceleration[j, i].x * textureColorMultiplier;
                    str = waveAcceleration[j, i].x * textureColorMultiplier;
                    if (str >= 0)
                        col[j + (i * DisplacementResolution)].r = str;
                    else
                        col[j + (i * DisplacementResolution)].g = -str;
                }
            }
        }


        //ripples blur
        if (UseSmoothWaves)
        {
            for (int i = 2; i < resMinus1; i++)
            {
                for (int j = 2; j < resMinus1; j++)
                {
                    //col[j + (i * DisplacementResolution)].r =
                    str = (wavePoints[j + (i * DisplacementResolution) - 2].y * 0.2f
                            + wavePoints[j + (i * DisplacementResolution) - 1].y * 0.4f
                            + wavePoints[j + (i * DisplacementResolution)].y * 0.6f
                            + wavePoints[j + (i * DisplacementResolution) + 1].y * 0.4f
                            + wavePoints[j + (i * DisplacementResolution) + 2].y * 0.2f
                            ) / 1.6f * textureColorMultiplier;
                    if (str >= 0)
                        col[j + (i * DisplacementResolution)].r = str;
                    else
                        col[j + (i * DisplacementResolution)].g = -str;
                }
            }
            for (int j = 2; j < resMinus1; j++)
            {
                for (int i = 2; i < resMinus1; i++)
                {
                    //col[i + (j * DisplacementResolution)].r =
                    str = (wavePoints[i + (j * DisplacementResolution) - 2].y * 0.2f
                            + wavePoints[i + (j * DisplacementResolution) - 1].y * 0.4f
                            + wavePoints[i + (j * DisplacementResolution)].y * 0.6f
                            + wavePoints[i + (j * DisplacementResolution) + 1].y * 0.4f
                            + wavePoints[i + (j * DisplacementResolution) + 2].y * 0.2f
                            ) / 1.6f * textureColorMultiplier;
                    if (str >= 0)
                        col[i + (j * DisplacementResolution)].r = str;
                    else
                        col[i + (j * DisplacementResolution)].g = -str;
                }
            }
        }
    }

    private void SetRippleTexture(int x, int y, float strength)
    {
        strength /= 100f;
        if (x >= 2 && x < DisplacementResolution - 2 && y >= 2 && y < DisplacementResolution - 2)
        {
            waveAcceleration[x, y].y -= strength;
            waveAcceleration[x + 1, y].y -= strength * 0.8f;
            waveAcceleration[x - 1, y].y -= strength * 0.8f;
            waveAcceleration[x, y + 1].y -= strength * 0.8f;
            waveAcceleration[x, y - 1].y -= strength * 0.8f;
            waveAcceleration[x + 1, y + 1].y -= strength * 0.7f;
            waveAcceleration[x + 1, y - 1].y -= strength * 0.7f;
            waveAcceleration[x - 1, y + 1].y -= strength * 0.7f;
            waveAcceleration[x - 1, y - 1].y -= strength * 0.7f;

            if (x >= 3 && x < DisplacementResolution - 3 && y >= 3 && y < DisplacementResolution - 3)
            {
                waveAcceleration[x + 2, y].y -= strength * 0.5f;
                waveAcceleration[x - 2, y].y -= strength * 0.5f;
                waveAcceleration[x, y + 2].y -= strength * 0.5f;
                waveAcceleration[x, y - 2].y -= strength * 0.5f;
            }
        }
    }

    private float GetTextureHeightByPosition(float x, float y)
    {
        x /= scaleBounds.x;
        y /= scaleBounds.y;
        x *= DisplacementResolution;
        y *= DisplacementResolution;
        if (x >= DisplacementResolution || y >= DisplacementResolution || x < 0 || y < 0)
            return 0;

        return waveAcceleration[(int)x, (int)y].x * textureColorMultiplier;
    }

    private Vector3 GerstnerOffset4(Vector2 xzVtx, Vector4 _GSteepness, Vector4 _GAmplitude, Vector4 _GFrequency, Vector4 _GSpeed, Vector4 _GDirectionAB, Vector4 _GDirectionCD)
    {
        Vector3 offsets = new Vector3();
        var stepAmpXX = _GSteepness.x * _GAmplitude.x;
        var stepAmpYY = _GSteepness.y * _GAmplitude.y;
        Vector4 AB = new Vector4(stepAmpXX * _GDirectionAB.x,
            stepAmpXX * _GDirectionAB.y,
            stepAmpYY * _GDirectionAB.z,
            stepAmpYY * _GDirectionAB.w);
        Vector4 CD = new Vector4(_GSteepness.z * _GAmplitude.z * _GDirectionCD.x,
            _GSteepness.z * _GAmplitude.z * _GDirectionCD.y,
            _GSteepness.w * _GAmplitude.w * _GDirectionCD.z,
            _GSteepness.w * _GAmplitude.w * _GDirectionCD.w);

        float dotA = Vector2.Dot(new Vector2(_GDirectionAB.x, _GDirectionAB.y), xzVtx);
        float dotB = Vector2.Dot(new Vector2(_GDirectionAB.z, _GDirectionAB.w), xzVtx);
        float dotC = Vector2.Dot(new Vector2(_GDirectionCD.x, _GDirectionCD.y), xzVtx);
        float dotD = Vector2.Dot(new Vector2(_GDirectionCD.z, _GDirectionCD.w), xzVtx);
        Vector4 dotABCD = new Vector4(dotA * _GFrequency.x, dotB * _GFrequency.y, dotC * _GFrequency.z, dotD * _GFrequency.w);

        Vector4 TIME = new Vector4((Time.time * _GSpeed.x) % 6.2831f,
            (Time.time * _GSpeed.y) % 6.2831f,
            (Time.time * _GSpeed.z) % 6.2831f,
            (Time.time * _GSpeed.w) % 6.2831f);

        Vector4 COS = new Vector4(Mathf.Cos(dotABCD.x + TIME.x),
            Mathf.Cos(dotABCD.y + TIME.y),
            Mathf.Cos(dotABCD.z + TIME.z),
            Mathf.Cos(dotABCD.w + TIME.w));
        Vector4 SIN = new Vector4(Mathf.Sin(dotABCD.x + TIME.x),
            Mathf.Sin(dotABCD.y + TIME.y),
            Mathf.Sin(dotABCD.z + TIME.z),
            Mathf.Sin(dotABCD.w + TIME.w));

        offsets.x = Vector4.Dot(COS, new Vector4(AB.x, AB.z, CD.x, CD.z));
        offsets.z = Vector4.Dot(COS, new Vector4(AB.y, AB.w, CD.y, CD.w));
        offsets.y = Vector4.Dot(SIN, _GAmplitude);

        return offsets;
    }
}