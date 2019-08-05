using System;
using System.Collections;
using UnityEngine;

public class ReflectionCamera : MonoBehaviour
{
    //public float m_ClipPlaneOffset = 0.07f;
    //public LayerMask CullingMask = ~(1 << 4);
    //public bool HDR;
    //public bool OcclusionCulling = true;
    //public float TextureScale = 1f;
    //public RenderTextureFormat RenderTextureFormat;
    //public FilterMode FilterMode = FilterMode.Point;
    //public RenderingPath RenderingPath;
    //public bool UseRealtimeUpdate;
    //public int FPSWhenMoveCamera = 40;
    //public int FPSWhenStaticCamera = 20;


    public LayerMask CullingMask = ~(1 << 4);
    public bool HDR;
    [Range(0.1f, 1)]
    public float TextureScale = 1f;

    private RenderTexture reflectionTexture;
    private GameObject goCam;
    public Camera reflectionCamera;
    Vector3 oldPos;

    static float ClipPlaneOffset = 0.07f;



    private void UpdateCamera(Camera cam)
    {
        CheckCamera(cam);

        //var cam = Camera.current;
        //if(cam==null) cam = Camera.main;
        if (cam == null) return;

        GL.invertCulling = true;

        Transform reflectiveSurface = transform; //waterHeight;

        Vector3 eulerA = cam.transform.eulerAngles;

        reflectionCamera.transform.eulerAngles = new Vector3(-eulerA.x, eulerA.y, eulerA.z);
        reflectionCamera.transform.position = cam.transform.position;

        Vector3 pos = reflectiveSurface.transform.position;
        pos.y = reflectiveSurface.position.y;
        Vector3 normal = reflectiveSurface.transform.up;
        float d = -Vector3.Dot(normal, pos) - ClipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        Matrix4x4 reflection = Matrix4x4.zero;
        reflection = CalculateReflectionMatrix(reflection, reflectionPlane);
        oldPos = cam.transform.position;
        Vector3 newpos = reflection.MultiplyPoint(oldPos);

        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

        Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);

        Matrix4x4 projection = cam.projectionMatrix;
        projection = CalculateObliqueMatrix(projection, clipPlane);
        reflectionCamera.projectionMatrix = projection;

        reflectionCamera.transform.position = newpos;
        Vector3 euler = cam.transform.eulerAngles;
        reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);


        reflectionCamera.Render();


        GL.invertCulling = false;
    }

    private void CheckCamera(Camera cam)
    {
        if (goCam == null)
        {
            reflectionTexture = new RenderTexture((int)(Screen.width * TextureScale), (int)(Screen.height * TextureScale), 16, RenderTextureFormat.Default);
            reflectionTexture.DiscardContents();
            goCam = new GameObject("Water Refl Camera");
            goCam.hideFlags = HideFlags.DontSave;
            goCam.transform.position = transform.position;
            goCam.transform.rotation = transform.rotation;

            reflectionCamera = goCam.AddComponent<Camera>();
            reflectionCamera.depth = cam.depth - 10;
            reflectionCamera.renderingPath = cam.renderingPath;
            reflectionCamera.depthTextureMode = DepthTextureMode.None; //todo check
            reflectionCamera.cullingMask = CullingMask;
            reflectionCamera.allowHDR = HDR;
            reflectionCamera.useOcclusionCulling = false;
            reflectionCamera.enabled = false;
            reflectionCamera.targetTexture = reflectionTexture;

            Shader.SetGlobalTexture("_ReflectionTex", reflectionTexture);
            //Camera.onPostRender += MyPostRender;
            //Camera.onPostRender += OnPostRender;
        }
    }

    static float Sgn(float a)
    {
        if (a > 0.0F)
        {
            return 1.0F;
        }
        if (a < 0.0F)
        {
            return -1.0F;
        }
        return 0.0F;
    }

    static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(
            Sgn(clipPlane.x),
            Sgn(clipPlane.y),
            1.0F,
            1.0F
            );
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        // third row = clip plane - fourth row
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];

        return projection;
    }

    static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1.0F - 2.0F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2.0F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2.0F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2.0F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2.0F * plane[1] * plane[0]);
        reflectionMat.m11 = (1.0F - 2.0F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2.0F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2.0F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2.0F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2.0F * plane[2] * plane[1]);
        reflectionMat.m22 = (1.0F - 2.0F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2.0F * plane[3] * plane[2]);

        reflectionMat.m30 = 0.0F;
        reflectionMat.m31 = 0.0F;
        reflectionMat.m32 = 0.0F;
        reflectionMat.m33 = 1.0F;

        return reflectionMat;
    }

    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * ClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }
    void SafeDestroy<T>(T component) where T : UnityEngine.Object
    {
        if (component == null) return;

        if (!Application.isPlaying)
            UnityEngine.Object.DestroyImmediate(component);
        else
            UnityEngine.Object.Destroy(component);
    }

    private void ClearCamera()
    {
        if (goCam)
        {
            SafeDestroy(goCam);
            goCam = null;
            //Camera.onPostRender -= MyPostRender;
        }
        if (reflectionTexture)
        {
            SafeDestroy(reflectionTexture);
            reflectionTexture = null;
        }
    }


    //// callback to be called after any camera finishes rendering
    //public void MyPostRender(Camera cam)
    //{
    //    if (cam == Camera.main)
    //    {
    //        Debug.Log("main");
    //        UpdateCamera(Camera.main);
    //    }
    //}

    public void OnWillRenderObject()
    {
            UpdateCamera(Camera.main);
    }

    //void OnRenderObject()
    //{

    //    if (Camera.current.name == "SceneCamera")
    //        if (!Application.isPlaying) UpdateCamera(Camera.current);
    //}

    private void OnEnable()
    {
    //    UpdateCamera(Camera.main);
    //    //Shader.EnableKeyword("editor_off");
        Shader.EnableKeyword("cubeMap_off");
    }

    private void OnDisable()
    {
        
        ClearCamera();
        //Shader.DisableKeyword("editor_off");
        Shader.DisableKeyword("cubeMap_off");
    }
}