using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct InteractionPoint
{
    public float x;
    public float y;
    public float z;
    public float radius;
    public float falloff;
    public float duration;
    public List<ParticleSystem> effects;
}

public class DynamicMap : MonoBehaviour
{
    public List<InteractionPoint> interactionPoints;

    private Material material;
    private new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = transform.GetComponent<Renderer>();
        material = renderer.material;
        interactionPoints = new List<InteractionPoint>();
    }

    // Update is called once per frame
    void Update()
    {
        // check for mouse input
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                InteractionPoint newPoint = new InteractionPoint();
                newPoint.x = hit.point.x;
                newPoint.y = hit.point.y;
                newPoint.z = hit.point.z;
                newPoint.radius = 2.50f;
                newPoint.falloff = .35f;
                interactionPoints.Add(newPoint);
            }
        }


        // adjust shader as needed
        if (interactionPoints.Count > 0)
        {
            float[] interactions = new float[500];

            for (int i = 0; i < interactionPoints.Count; i++)
            {
                interactions[i * 5] = interactionPoints[i].x;
                interactions[i * 5 + 1] = interactionPoints[i].y;
                interactions[i * 5 + 2] = interactionPoints[i].z;
                interactions[i * 5 + 3] = interactionPoints[i].radius;
                interactions[i * 5 + 4] = interactionPoints[i].falloff;
            }

            material.SetFloatArray("_InteractionPoints", interactions);
            material.SetInt("_NumInteractions", interactionPoints.Count);
        
            renderer.material = material;

        }

        Debug.Log(interactionPoints.Count);
    }
}
