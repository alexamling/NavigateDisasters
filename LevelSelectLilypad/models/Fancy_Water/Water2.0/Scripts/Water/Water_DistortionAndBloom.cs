using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]

#if UNITY_5_4_OR_NEWER
//[ImageEffectAllowedInSceneView]
#endif
public class Water_DistortionAndBloom: MonoBehaviour
{
    [Range(0.05f, 1)]
    [Tooltip("Camera render texture resolution")]
    public float RenderTextureResolutoinFactor = 0.5f;
    public LayerMask CullingMask = ~(1 << 4);
    // public bool UseBloom = false;

    //[Range(0.1f, 3)]
    //[Tooltip("Filters out pixels under this level of brightness.")]
    //public float Threshold = 2f;

    //[SerializeField, Range(0, 1)]
    //[Tooltip("Makes transition between under/over-threshold gradual.")]
    //public float SoftKnee = 0f;

    //[Range(1, 7)]
    //[Tooltip("Changes extent of veiling effects in A screen resolution-independent fashion.")]
    //public float Radius = 7;

    //[Tooltip("Blend factor of the result image.")]
    //public float Intensity = 1;

    //[Tooltip("Controls filter quality and buffer resolution.")]
    //public bool HighQuality;


    //[Tooltip("Reduces flashing noise with an additional filter.")]
    //public bool AntiFlicker;

    //const string shaderName = "Hidden/KriptoFX/PostEffects/RFX1_Bloom";
    //const string shaderAdditiveName = "Hidden/KriptoFX/PostEffects/RFX1_BloomAdditive";
    //const string cameraName = "MobileCamera(Distort_Bloom_Depth)";

    RenderTexture source;
    RenderTexture depth;
    RenderTexture destination;
    private int previuosFrameWidth, previuosFrameHeight;
    private float previousScale;
    private Camera addCamera;
    private GameObject tempGO;
    private bool HDRSupported;

    //private Material m_Material;

    //public Material mat
    //{
    //    get
    //    {
    //        if (m_Material == null)
    //            m_Material = CheckShaderAndCreateMaterial(Shader.Find(shaderName));

    //        return m_Material;
    //    }
    //}

   // private Material m_MaterialAdditive;

    //public Material matAdditive
    //{
    //    get
    //    {
    //        if (m_MaterialAdditive == null)
    //        {
    //            m_MaterialAdditive = CheckShaderAndCreateMaterial(Shader.Find(shaderAdditiveName));
    //            m_MaterialAdditive.renderQueue = 3900;
    //        }

    //        return m_MaterialAdditive;
    //    }
    //}

    public static Material CheckShaderAndCreateMaterial(Shader s)
    {
        if (s == null || !s.isSupported)
            return null;

        var material = new Material(s);
        material.hideFlags = HideFlags.DontSave;
        return material;
    }

    #region Private Members

    private const int kMaxIterations = 16;
    private readonly RenderTexture[] m_blurBuffer1 = new RenderTexture[kMaxIterations];
    private readonly RenderTexture[] m_blurBuffer2 = new RenderTexture[kMaxIterations];

    private void OnDisable()
    {
        //if (m_Material != null)
        //    DestroyImmediate(m_Material);
        //m_Material = null;

        //if (m_MaterialAdditive != null)
        //    DestroyImmediate(m_MaterialAdditive);
        //m_MaterialAdditive = null;

        if(tempGO != null)
            DestroyImmediate(tempGO);

        Shader.DisableKeyword("DISTORT_OFF");
        Shader.DisableKeyword("_MOBILEDEPTH_ON");
    }

    //private void OnGUI()
    //{
    //    if (Event.current.type.Equals(EventType.Repaint))
    //    {
    //        if (UseBloom && HDRSupported && destination != null) Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), destination, matAdditive);
    //    }
    //    GUI.Label(new Rect(250, 0, 30, 30), "HDR: " + HDRSupported, guiStyleHeader);
    //}
    GUIStyle guiStyleHeader = new GUIStyle();

    void Start()
    {
        InitializeRenderTarget();
    }

    void LateUpdate()
    {
        if (previuosFrameWidth != Screen.width || previuosFrameHeight != Screen.height || Mathf.Abs(previousScale - RenderTextureResolutoinFactor) > 0.01f)
        {
            InitializeRenderTarget();
            previuosFrameWidth = Screen.width;
            previuosFrameHeight = Screen.height;
            previousScale = RenderTextureResolutoinFactor;
        }
        Shader.EnableKeyword("DISTORT_OFF");
        Shader.EnableKeyword("_MOBILEDEPTH_ON");
        GrabImage();
       // if (UseBloom && HDRSupported) UpdateBloom();
        Shader.SetGlobalTexture("_GrabTexture", source);
        Shader.SetGlobalTexture("_CameraDepthTexture", depth);
        Shader.SetGlobalFloat("_GrabTextureScale", RenderTextureResolutoinFactor);
        Shader.DisableKeyword("DISTORT_OFF");
    }

    private void InitializeRenderTarget()
    {
        var width = (int)(Screen.width * RenderTextureResolutoinFactor);
        var height = (int)(Screen.height * RenderTextureResolutoinFactor);
        //if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB111110Float))
        //{
        //    source = new RenderTexture(width, height, 0, RenderTextureFormat.RGB111110Float);
        //    depth = new RenderTexture(width, height, 8, RenderTextureFormat.Depth);
        //    HDRSupported = true;
        //}
        //else
        {
            HDRSupported = false;
            source = new RenderTexture(width, height, 0, RenderTextureFormat.RGB565);
            depth = new RenderTexture(width, height, 8, RenderTextureFormat.Depth);
        }
    }


    void GrabImage()
    {
        var cam = Camera.current;
        if (cam == null) cam = Camera.main;
       
        if (tempGO == null)
        {
            tempGO = new GameObject();
            tempGO.hideFlags = HideFlags.HideAndDontSave;
            //tempGO.name = cameraName;
            addCamera = tempGO.AddComponent<Camera>();
            addCamera.enabled = false;
           // addCamera.transform.parent = cam.transform;
        }
        else addCamera = tempGO.GetComponent<Camera>();
        addCamera.CopyFrom(cam);
        addCamera.SetTargetBuffers(source.colorBuffer, depth.depthBuffer);
        addCamera.depth--;
        //addCamera.targetTexture = source;
        addCamera.cullingMask = CullingMask;
        addCamera.Render();


        //var cam = Camera.current;
        //if (cam != null && Camera.current.name == "SceneCamera")
        //{
        //    tempGO = GameObject.Find("MobileSceneCamera(Distort_Bloom_Depth)");
        //    if (tempGO == null)
        //    {
        //        tempGO = new GameObject();
        //        //tempGO.hideFlags = HideFlags.HideAndDontSave;
        //        tempGO.name = "MobileSceneCamera(Distort_Bloom_Depth)";
        //        addCamera = tempGO.AddComponent<Camera>();
        //        addCamera.enabled = false;

        //    }
        //    else addCamera = tempGO.GetComponent<Camera>();
        //    addCamera.CopyFrom(cam);
        //    addCamera.targetTexture = source;
        //    addCamera.SetTargetBuffers(source.colorBuffer, depth.depthBuffer);
        //    addCamera.Render();
        //    return;
        //}


        //if (tempGO == null)
        //{
        //    tempGO = new GameObject();
        //    //tempGO.hideFlags = HideFlags.HideAndDontSave;
        //    tempGO.name = "MobileCamera(Distort_Bloom_Depth)";
        //    addCamera = tempGO.AddComponent<Camera>();
        //    addCamera.CopyFrom(Camera.main);
        //    addCamera.transform.parent = Camera.main.transform;
        //    addCamera.targetTexture = source;
        //    addCamera.enabled = false;
        //}
        //addCamera.SetTargetBuffers(source.colorBuffer, depth.depthBuffer);
        //addCamera.Render();

    }

    #endregion
}