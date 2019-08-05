using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float Density = 700;
    public int SlicesPerAxis = 2;
    public bool IsConcave = false;
    public int VoxelsLimit = 16;
    public float WaveVelocity = 0.05f;

    private const float Dampfer = 0.1f;
    private const float WaterDensity = 1000;

    private float voxelHalfHeight;
    private float localArchimedesForce;
    private List<Vector3> voxels;
    private bool isMeshCollider;
    private List<Vector3[]> forces;
    private WaterRipples waterRipples;
    private Rigidbody rb;


    /// <summary>
    /// Provides initialization.
    /// </summary>
    private void Start()
    {
        forces = new List<Vector3[]>();
        rb = GetComponent<Rigidbody>();
        var originalRotation = transform.rotation;
        var originalPosition = transform.position;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
            Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no collider. MeshCollider has been added.", name));
        }
        isMeshCollider = GetComponent<MeshCollider>() != null;

        var bounds = GetComponent<Collider>().bounds;
        if (bounds.size.x < bounds.size.y)
        {
            voxelHalfHeight = bounds.size.x;
        }
        else
        {
            voxelHalfHeight = bounds.size.y;
        }
        if (bounds.size.z < voxelHalfHeight)
        {
            voxelHalfHeight = bounds.size.z;
        }
        voxelHalfHeight /= 2 * SlicesPerAxis;

        // The object must have a RidigBody
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().mass = 1f; ;
            Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no Rigidbody. Rigidbody has been added.", name));
        }
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -bounds.extents.y * 0f, 0) + transform.InverseTransformPoint(bounds.center);
        
        voxels = SliceIntoVoxels(isMeshCollider && IsConcave);

        transform.rotation = originalRotation;
        transform.position = originalPosition;


        float volume = GetComponent<Rigidbody>().mass / Density;

        WeldPoints(voxels, VoxelsLimit);

        float archimedesForceMagnitude = WaterDensity * Mathf.Abs(Physics.gravity.y) * volume;
        localArchimedesForce = archimedesForceMagnitude / voxels.Count;
    }

    private List<Vector3> SliceIntoVoxels(bool concave)
    {
        var points = new List<Vector3>(SlicesPerAxis * SlicesPerAxis * SlicesPerAxis);

        if (concave)
        {
            var meshCol = GetComponent<MeshCollider>();

            var convexValue = meshCol.convex;
            meshCol.convex = false;

            var bounds = GetComponent<Collider>().bounds;
            for (int ix = 0; ix < SlicesPerAxis; ix++)
            {
                for (int iy = 0; iy < SlicesPerAxis; iy++)
                {
                    for (int iz = 0; iz < SlicesPerAxis; iz++)
                    {
                        float x = bounds.min.x + bounds.size.x / SlicesPerAxis * (0.5f + ix);
                        float y = bounds.min.y + bounds.size.y / SlicesPerAxis * (0.5f + iy);
                        float z = bounds.min.z + bounds.size.z / SlicesPerAxis * (0.5f + iz);

                        var p = transform.InverseTransformPoint(new Vector3(x, y, z));

                        if (PointIsInsideMeshCollider(meshCol, p))
                        {
                            points.Add(p);
                        }
                    }
                }
            }
            if (points.Count == 0)
            {
                points.Add(bounds.center);
            }

            meshCol.convex = convexValue;
        }
        else
        {
            var bounds = GetComponent<Collider>().bounds;
            for (int ix = 0; ix < SlicesPerAxis; ix++)
            {
                for (int iy = 0; iy < SlicesPerAxis; iy++)
                {
                    for (int iz = 0; iz < SlicesPerAxis; iz++)
                    {
                        float x = bounds.min.x + bounds.size.x / SlicesPerAxis * (0.5f + ix);
                        float y = bounds.min.y + bounds.size.y / SlicesPerAxis * (0.5f + iy);
                        float z = bounds.min.z + bounds.size.z / SlicesPerAxis * (0.5f + iz);

                        var p = transform.InverseTransformPoint(new Vector3(x, y, z));

                        points.Add(p);
                    }
                }
            }
        }

        return points;
    }

    private static bool PointIsInsideMeshCollider(Collider c, Vector3 p)
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        foreach (var ray in directions)
        {
            RaycastHit hit;
            if (c.Raycast(new Ray(p - ray * 1000, ray), out hit, 1000f) == false)
            {
                return false;
            }
        }

        return true;
    }

    private static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
    {
        float minDistance = float.MaxValue, maxDistance = float.MinValue;
        firstIndex = 0;
        secondIndex = 1;

        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                float distance = Vector3.Distance(list[i], list[j]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    firstIndex = i;
                    secondIndex = j;
                }
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
            }
        }
    }

    private static void WeldPoints(IList<Vector3> list, int targetCount)
    {
        if (list.Count <= 2 || targetCount < 2)
        {
            return;
        }

        while (list.Count > targetCount)
        {
            int first, second;
            FindClosestPoints(list, out first, out second);

            var mixed = (list[first] + list[second]) * 0.5f;
            list.RemoveAt(second); // the second index is always greater that the first => removing the second item first
            list.RemoveAt(first);
            list.Add(mixed);
        }
    }


    Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        var side1 = b - a;
        var side2 = c - a;
        return Vector3.Cross(side1, side2).normalized;
    }

    private void FixedUpdate()
    {
        if (waterRipples==null)
            return;
        forces.Clear();
        var length = voxels.Count;
        var pointCache = new Vector3[length];
        for (int i = 0; i < length; ++i)
        {
            var wp = transform.TransformPoint(voxels[i]);
            pointCache[i] = waterRipples.GetOffsetByPosition((wp));
        }
        var normal = (GetNormal(pointCache[0], pointCache[1], pointCache[2])*WaveVelocity + Vector3.up).normalized;
        for(int i=0; i<length; ++i)
        {
            var wp = transform.TransformPoint(voxels[i]);
            float waterLevel = pointCache[i].y;
            //TODO normal if 1 point.
            

            if (wp.y - voxelHalfHeight < waterLevel)
            {
                float k = (waterLevel - wp.y)/(2*voxelHalfHeight) + 0.5f;
                if (k > 1)
                {
                    k = 1f;
                }
                else if (k < 0)
                {
                    k = 0f;
                }

                var velocity = GetComponent<Rigidbody>().GetPointVelocity(wp);
                var localDampingForce = -velocity*Dampfer*GetComponent<Rigidbody>().mass;

                Vector3 force = localDampingForce + Mathf.Sqrt(k)*(normal*localArchimedesForce);
                //Debug.DrawRay(wp, Mathf.Sqrt(k)*(WaterHit.normal*localArchimedesForce), Color.blue);

                GetComponent<Rigidbody>().AddForceAtPosition(force, wp);

                forces.Add(new[] {wp, force}); // For drawing force gizmos


            }
        }


        //rb.rotation = Quaternion.Euler(Mathf.SmoothStep(-5, 5, Time.deltaTime), rb.rotation.eulerAngles.y, Mathf.SmoothStep(-5, 5, Time.deltaTime));


    }

    private void OnDrawGizmos()
    {
        if (voxels == null || forces == null)
        {
            return;
        }

        const float gizmoSize = 0.05f;
        Gizmos.color = Color.yellow;

        foreach (var p in voxels)
        {
            Gizmos.DrawCube(transform.TransformPoint(p), new Vector3(gizmoSize, gizmoSize, gizmoSize));
        }

        Gizmos.color = Color.cyan;

        foreach (var force in forces)
        {
            Gizmos.DrawCube(force[0], new Vector3(gizmoSize, gizmoSize, gizmoSize));
            Gizmos.DrawLine(force[0], force[0] + force[1] / GetComponent<Rigidbody>().mass);
        }
    }

    void OnTriggerEnter(Collider collidedObj)
    {
        var temp = collidedObj.GetComponent<WaterRipples>();
        if (temp!=null)
            waterRipples=temp;
    }

    void OnEnable()
    {
        waterRipples = null;
    }
}